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

let httpResponseHeaders (suaveHeaders : (string * string) list) (headers : HttpResponseHeaders) =
  suaveHeaders |> List.iter headers.Add
  headers

let httpResponseMessage (httpResult : HttpResult) =
  let content = function
  | Bytes c -> c
  | _ -> Array.empty

  let res = new HttpResponseMessage()
  res.Content <- new ByteArrayContent(content httpResult.content)
  res.StatusCode <- httpStatusCode httpResult.status
  httpResponseHeaders httpResult.headers res.Headers |> ignore
  res

let suaveHttpResult (httpResponseMessage : HttpResponseMessage) = async {
  let statusCode =
    match suaveHttpCode httpResponseMessage.StatusCode with
    | Some x -> x
    | _ -> HTTP_502 
  let! content = async {
    if isNull httpResponseMessage.Content then 
      return NullContent 
    else
      let! res = httpResponseMessage.Content.ReadAsByteArrayAsync() |> Async.AwaitTask
      return Bytes res
  }
    
  return {
    status = statusCode
    headers = suaveHttpResponseHeaders httpResponseMessage.Headers
    content = content
    writePreamble = false
  }
}
  
