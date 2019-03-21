namespace FGit

open System.IO

module Utils = 
  // small but very useful definition
  type Path = string
  let inline (/) (o1:string) (o2:string) :Path = Path.Combine(o1, o2)
