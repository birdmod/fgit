namespace FGit

open System.Security.Cryptography
open System.IO
open System.IO.Compression
open System.Text
open FGit.Utils

module Add = 

  let Execute (parameters: string[]) = 
    let currentDir = Directory.GetCurrentDirectory()

    let computeFileHash fileName = 
      let filePath = currentDir/fileName
      if (File.Exists filePath) then
        let data = File.ReadAllBytes(filePath)
        let sha = new SHA1CryptoServiceProvider()
        let hash = 
          (new StringBuilder(), sha.ComputeHash(data))
          ||> Array.fold (fun (acc:StringBuilder) item -> acc.Append(item.ToString("X2")))
        Some(fileName, hash.ToString().Replace("-", ""))
      else
        None

    let compressFile fileName (hash:string) =
      let filePath =  currentDir/fileName
      if (File.Exists filePath) then
        let fileContent = File.ReadAllBytes(filePath)

        if (not <| Directory.Exists(currentDir/".fgit"/"objects"/hash.[0..1])) then
          Directory.CreateDirectory(currentDir/".fgit"/"objects"/hash.[0..1]) |> ignore

        use outputStream = new FileStream(currentDir/".fgit"/"objects"/hash.[0..1]/hash.[2..], FileMode.Create)
        use gzStream = new GZipStream(outputStream, CompressionMode.Compress)
        gzStream.Write(fileContent, 0, fileContent.Length)

    let writeIntoIndex fileName hash =
      let indexPath = currentDir/".fgit"/"index"
      File.AppendAllText(indexPath, sprintf "%s %s" fileName hash)

    let addToStaging fileName = 
      let tpl = computeFileHash fileName
      match tpl with
      | None -> ()
      | Some(fName, hash) -> compressFile fName hash
                             printfn "fgit add: Compressed %s" fName
                             writeIntoIndex fName hash

    if (not <| Directory.Exists (currentDir/".fgit")) then
      printfn "Not an RGit project"
      1
    else
      Array.iter addToStaging parameters
      0

