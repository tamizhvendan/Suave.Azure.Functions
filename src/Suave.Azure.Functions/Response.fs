module Suave.Azure.Functions.Response
  
open System.Net
open Suave.Http
open System.Net.Http
open System.Net.Http.Headers

let httpStatusCode (httpCode : HttpCode) : HttpStatusCode = 
  LanguagePrimitives.EnumOfValue httpCode.code

let suaveHttpCode (httpStatusCode : HttpStatusCode) =
  match LanguagePrimitives.EnumToValue httpStatusCode |> HttpCode.tryParse with
  | Choice1Of2 httpCode -> Some httpCode
  | _ -> None
  
let suaveHttpResponseHeaders (httpResponseHeaders : HttpResponseHeaders) =
  httpResponseHeaders
  |> Seq.map (fun h -> h.Key, h.Value)
  |> Seq.collect (fun (k,vs) -> vs |> Seq.map (fun v -> (k,v)))
  |> Seq.toList

let httpResponseHeaders (suaveHeaders : (string * string) list) =
  let res = new HttpResponseMessage()
  suaveHeaders |> List.iter res.Headers.Add
  res.Headers