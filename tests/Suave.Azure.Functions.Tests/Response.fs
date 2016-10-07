module Suave.Azure.Functions.Tests.Response

open Suave.Azure.Functions
open Suave.Http
open System.Net
open Xunit
open FsCheck
open FsCheck.Xunit
open System.Net.Http
open TestUtil

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
let ``httpResponseHeaders maps Suave.Http's HttpHeaders to System.Net.Http's HttpResponseHeaders ``()=
  let suaveHeaders = [
    "X-Test1" , "1"
    "X-Test2",  "2"
    "X-Test2", "22"
    "X-Test3", "3"
  ]
  let httpResponseHeaders = Response.httpResponseHeaders suaveHeaders |> Seq.map (fun h -> (h.Key, h.Value))  
  suaveHeaders |> List.forall (contains httpResponseHeaders) |> Assert.True
  Assert.Equal(3, httpResponseHeaders |> Seq.length)