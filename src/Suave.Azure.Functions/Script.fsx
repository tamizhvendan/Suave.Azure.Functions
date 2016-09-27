#r "./../../packages/Suave/lib/net40/Suave.dll"
#r "./../../packages/System.Net.Http/lib/net46/System.Net.Http.dll"
open Suave.Http
open System.Net
open FSharp.Reflection

let unionCaseByName<'a> name = 
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name = name) with
    |[|case|] -> Some case
    |_ -> None

unionCaseByName<HttpCode> "HTTP_201"