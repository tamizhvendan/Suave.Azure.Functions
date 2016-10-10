module Suave.Azure.Functions.Tests.TestUtil

open System.Threading.Tasks

let values headers key =
   headers |> Seq.filter (fun x-> fst x = key) |> Seq.map snd |> Seq.toList

let contains headers (key,value) =
  if key = "" then true else
    let header = headers |> Seq.find (fun (k,_) -> k = key)
    let values = snd header
    values |> Seq.contains value

let runTask<'T> (asyncContent : Task<'T>) = 
  asyncContent |> Async.AwaitTask |> Async.RunSynchronously