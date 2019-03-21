open System
open FGit

[<EntryPoint>]
let main argv =
  if (Array.length argv = 0) then
    printfn "Usage: fgit <command> [<args>]"; 1
  else
    let commandToExecute = argv.[0]
    match commandToExecute with 
    | "init" -> Init.Execute()
    | "add" -> Add.Execute argv.[1..]
    | _ -> printfn "No such command"; 1
