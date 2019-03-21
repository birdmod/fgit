namespace FGit

open FGit.Utils
open Newtonsoft.Json
open System
open System.IO
open System.Text
open System.IO.Compression
open System.Security.Cryptography

module Commit = 
  type IndexRecord = {
    sha1: string;
    filePath: string
  }

  type CommitRecord = {
    commiter: string
    tree: IndexRecord array
    message: string
  }

  let Execute (parameters: string[]) = 
    let commitMessage = parameters.[0]
    let currentDir = Directory.GetCurrentDirectory()

    let clearStagingArea() = 
      File.WriteAllText("", currentDir/".rgit"/"index")

    let getIndexLines() = 
      let transform (s:string) = 
        let indexLineParts = s.Split(" ")
        {sha1 = indexLineParts.[1]; filePath = indexLineParts.[0]}
      File.ReadAllLines (currentDir/".fgit"/"index") |> Array.map transform

    let buildCommitObj (modifications:IndexRecord array) = 
      // should be hash of all hashes
      // we should be able to configure the user name at least
      let commitIdentifier = System.Text.Encoding.Unicode.GetBytes(DateTime.UtcNow.ToString() + "userToConfigure")
      let shaProvider = new SHA1CryptoServiceProvider()
      let hashCommitIdentifier = 
        (new StringBuilder(), shaProvider.ComputeHash(commitIdentifier))
        ||> Array.fold (fun (acc:StringBuilder) item -> acc.Append(item.ToString("X2")))
        |> fun sb -> sb.ToString()

      let commit = {
        commiter = "userToConfigure"
        tree = modifications
        message = commitMessage
      }
      let commitContent = System.Text.Encoding.Unicode.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(commit))

      use outputStream = new FileStream(currentDir/".fgit"/"objects"/hashCommitIdentifier.[0..1]/hashCommitIdentifier.[2..], FileMode.Create)
      use gzStream = new GZipStream(outputStream, CompressionMode.Compress)
      gzStream.Write(commitContent, 0, commitContent.Length)

    match (Directory.Exists (currentDir/".rgit")), getIndexLines() with
    | false, _ -> System.Console.WriteLine "Not an RGit project"; 1
    | true, lines when Array.length lines = 0 -> System.Console.WriteLine "Nothing to commit"; 1
    | true, lines -> buildCommitObj lines
                     clearStagingArea()
                     0