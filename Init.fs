namespace FGit

open System.IO
open FGit.Utils

module Init = 
  let Execute () = 

    let createFolder (failMessage:string option) folderName = 
      let folderPath = Directory.GetCurrentDirectory()/folderName
      if Directory.Exists folderPath then
        failMessage |> Option.iter (fun msg -> printfn "%s" msg)
      else
        Directory.CreateDirectory folderPath |> ignore

    createFolder (Some("Existing RGit project")) ".fgit" 

    let createFolderSimple = createFolder None
    createFolderSimple (".fgit"/"config")
    createFolderSimple (".fgit"/"objects"/"info") 
    createFolderSimple (".fgit"/"objects"/"pack")
    createFolderSimple (".fgit"/"refs"/"heads")
    createFolderSimple (".fgit"/"refs"/"tags")
    
    let headFilePath = (Directory.GetCurrentDirectory()/".fgit"/"HEAD")
    use headFile = File.CreateText headFilePath
    headFile.WriteLine "ref: refs/heads/master"

    System.Console.WriteLine "RGit initialized in .fgit" 

    0