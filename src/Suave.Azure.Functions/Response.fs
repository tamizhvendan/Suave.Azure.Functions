module Suave.Azure.Functions.Response
  
open System.Net
open Suave.Http

let httpStatusCode (httpCode : HttpCode) : HttpStatusCode = 
  LanguagePrimitives.EnumOfValue httpCode.code

let suaveHttpCode (httpStatusCode : HttpStatusCode) =
  match LanguagePrimitives.EnumToValue httpStatusCode |> HttpCode.tryParse with
  | Choice1Of2 httpCode -> Some httpCode
  | _ -> None
  