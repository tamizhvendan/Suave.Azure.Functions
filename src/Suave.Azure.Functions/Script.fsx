#r "./../../packages/Suave/lib/net40/Suave.dll"
#r "./../../packages/System.Net.Http/lib/net46/System.Net.Http.dll"
open Suave.Http
open System.Net
open FSharp.Reflection

let unionCase<'a> name value = 
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name = name) with
    |[|case|] -> 
        FSharpValue.MakeUnion(case, value) :?> 'a |> Some
    |_ -> None

unionCase<HttpCode> "HTTP_201" [||]

