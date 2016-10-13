module Suave.Azure.Functions.Tests.Response

open Suave.Azure.Functions
open Suave.Http
open System.Net
open Xunit
open FsCheck
open FsCheck.Xunit
open System.Net.Http
open TestUtil
open System.Text
open xunit.jet.Assert

let suaveHeaders = [
    "X-Test1" , "1"
    "X-Test2",  "2"
    "X-Test2", "22"
    "X-Test3", "3"
  ]

[<Property>]
let ``httpStatusCode maps Suave's HttpCode to System.Net's HttpStatusCode `` (httpCode : HttpCode) =
  let httpStatusCode = Response.httpStatusCode httpCode
  LanguagePrimitives.EnumToValue httpStatusCode = httpCode.code

[<Property>]
let ``suaveHttpCode maps System.Net's HttpStatusCode to Suave's HttpCode`` (httpStatusCode : HttpStatusCode) = 
  let httpCode = Response.suaveHttpCode httpStatusCode
  let code = LanguagePrimitives.EnumToValue httpStatusCode
  let unSupportedSuaveHttpCodes = [306;426]

  if List.contains code unSupportedSuaveHttpCodes then
    Option.isNone httpCode
  else
    Option.isSome httpCode && httpCode.Value.code = code


[<Fact>]
let ``suaveHttpResponseHeaders maps System.Net.Http's HttpResponseHeaders to Suave.Http's HttpHeaders``()=
  let content = new HttpResponseMessage()
  content.Headers.Add("X-Test2", ["2"; "22"])
  content.Headers.Add("X-Test1", "1")
  let suaveHeaders = Response.suaveHttpResponseHeaders content.Headers
  let values = values suaveHeaders
  let expected = content.Headers |> Seq.map (fun h -> h.Key, (h.Value |> Seq.toList))
  expected
  |> Seq.iter (fun (key,vs) -> Assert.True(values key = vs))

[<Fact>]
let ``httpResponseMessage maps Suave's HttpResult to HttpResponseMessage``() =
  let responseBody = """{ "x" : "y" }"""
  
  let suaveHttpResult = 
    {
      status = HTTP_200
      headers = suaveHeaders
      content = Bytes (Encoding.UTF8.GetBytes responseBody)
      writePreamble = false
    }
  let httpResponseMessage = Response.httpResponseMessage suaveHttpResult
  let actualContent = runTask <| httpResponseMessage.Content.ReadAsStringAsync()
  equalDeep responseBody actualContent
  equalDeep httpResponseMessage.StatusCode HttpStatusCode.OK
  let headers = 
   httpResponseMessage.Content.Headers 
   |> Seq.map (fun h -> h.Key, h.Value)
   |> Seq.collect (fun (k,vs) -> vs |> Seq.map (fun v -> (k,v)))
   |> Seq.toList
  suaveHeaders
  |> List.iter (fun (k,v) -> headers |> List.filter (fun x -> (fst x) = k) |> List.contains (k,v) |> equalDeep true)


[<Fact>]
let ``suaveHttpResult maps HttpResponseMessage to Suave's HttpResult`` () =
  let responseBody = """{ "x" : "y" }"""
  let res = new HttpResponseMessage(HttpStatusCode.Created)
  res.Content <- new StringContent(responseBody)
  suaveHeaders |> List.iter res.Headers.Add  

  let suaveHttpResult = Response.suaveHttpResult res |> Async.RunSynchronously
  let content = 
    match suaveHttpResult.content with
    | Bytes xs -> xs
    | _ -> Array.empty
  equalDeep suaveHeaders suaveHttpResult.headers 
  equalDeep HTTP_201 suaveHttpResult.status
  equalDeep responseBody (Encoding.UTF8.GetString content)
