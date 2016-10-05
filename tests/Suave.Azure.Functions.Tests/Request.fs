module Suave.Azure.Functions.Tests.Request

open Suave.Http
open Xunit
open FsCheck
open FsCheck.Xunit
open Suave.Azure.Functions
open System.Net.Http
open System.Collections.Generic


let httpMethods : IEnumerable<obj[]> = 
  [
   HttpMethod.Get; HttpMethod.Post; HttpMethod.Put; HttpMethod.Delete
   HttpMethod.Head; HttpMethod.Trace; HttpMethod.Options;
   new HttpMethod("Patch"); new HttpMethod("Other"); new HttpMethod("Connect")
  ] 
  |> List.map (fun m -> m :> obj)
  |> List.map Array.singleton
  |> List.toSeq
let toLower (str : System.String) = str.ToLowerInvariant()

[<Theory>]
[<MemberData("httpMethods")>]
let ``suaveHttpMethod maps System.Net.Http's HttpMethod to Suave.Http's HttpMethod`` (httpMethod : HttpMethod) =
  
  let suaveHttpMethod = Request.suaveHttpMethod httpMethod
  Assert.Equal(httpMethod |> string |> toLower, suaveHttpMethod |> string |> toLower)

[<Property>]
let ``httpMethod maps Suave.Http's HttpMethod to System.Net.Http's HttpMethod`` (suaveHttpMethod : Suave.Http.HttpMethod) =
  let httpMethod = Request.httpMethod suaveHttpMethod
  match suaveHttpMethod with
  | OTHER s -> Option.isNone httpMethod
  | _ -> 
    Option.isSome httpMethod && 
      httpMethod.Value.Method
      |> toLower
      |> (=) (suaveHttpMethod |> string |> toLower)