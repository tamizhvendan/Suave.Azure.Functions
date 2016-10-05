#I "./../../packages"
#r "Suave/lib/net40/Suave.dll"
#r "System.Net.Http/lib/net46/System.Net.Http.dll"
open Suave.Http
open System.Net
open System.Globalization
open System.Net.Http

let httpMethod (suaveHttpMethod : Suave.Http.HttpMethod) =  
  match suaveHttpMethod with
  | OTHER _ -> 
    None
  | _ -> 
    let method : System.String = suaveHttpMethod |> string
    let normalized = method.ToLowerInvariant()
    string(System.Char.ToUpper(normalized.[0])) + normalized.Substring(1)
    |> HttpMethod |> Some