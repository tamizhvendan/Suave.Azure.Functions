module Suave.Azure.Functions.Tests.Request

open Suave.Http
open Xunit
open FsCheck
open FsCheck.Xunit
open Suave.Azure.Functions
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Collections.Generic
open TestUtil
open System.Text

let toMemberData x : IEnumerable<obj[]> = 
  (List.map (fun m -> m :> obj)
  >> List.map Array.singleton
  >> List.toSeq) x

let httpMethods = 
  [
   HttpMethod.Get; HttpMethod.Post; HttpMethod.Put; HttpMethod.Delete
   HttpMethod.Head; HttpMethod.Trace; HttpMethod.Options;
   new HttpMethod("Patch"); new HttpMethod("Other"); new HttpMethod("Connect")
  ] 
  |> toMemberData



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

[<Fact>]
let ``suaveHttpRequestHeaders maps System.Net.Http's HttpRequestHeaders to Suave.Http's HttpHeaders`` () =
 let request = new System.Net.Http.HttpRequestMessage()
 request.Headers.Add("X-Test1", "1")
 request.Headers.Add("X-Test2", ["2"; "22"])
 let suaveHeaders = Request.suaveHttpRequestHeaders request.Headers
 let values = values suaveHeaders
 let expected = request.Headers |> Seq.map (fun h -> h.Key, (h.Value |> Seq.toList))
 expected
 |> Seq.iter (fun (key,vs) -> Assert.True(values key = vs))

[<Fact>]
let ``httpRequestHeaders maps Suave.Http's HttpHeaders to System.Net.Http's HttpRequestHeaders ``() =
  let suaveHeaders = [
    "X-Test1" , "1"
    "X-Test2",  "2"
    "X-Test2", "22"
  ]
  let requestHeaders = Request.httpRequestHeaders suaveHeaders |> Seq.map (fun h -> (h.Key, h.Value))  
  suaveHeaders |> List.forall (contains requestHeaders) |> Assert.True
  Assert.Equal(2, requestHeaders |> Seq.length)

[<Fact>]
let ``suaveRawForm maps System.Net.Http's HttpContent to byte array``() =
  let body = """{ "foo" : "bar"}"""
  let content = new StringContent(body) :> HttpContent  
  let actual = Request.suaveRawForm content |> Async.RunSynchronously
  Assert.Equal(body, Encoding.UTF8.GetString(actual))

[<Fact>]
let ``httpContent map Suave's RawForm Value to System.Net.Http's HttpContent``() =
  let body = """{ "foo" : "bar"}"""
  let rawForm = Encoding.UTF8.GetBytes body
  let actual = 
    (Request.httpContent rawForm).ReadAsByteArrayAsync()
    |> Async.AwaitTask
    |> Async.RunSynchronously
  Assert.Equal(body, Encoding.UTF8.GetString(actual))
    
  
  

