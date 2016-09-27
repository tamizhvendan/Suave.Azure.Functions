module Suave.Azure.Functions.Tests

open Suave.Azure.Functions
open Suave.Http
open System.Net
open FsCheck
open FsCheck.Xunit

[<Property>]
let ``Suave HttpStatusCode Should Map to System.Net's HttpStatusCode `` (httpCode : HttpCode) =
  let httpStatusCode = Response.httpStatusCode httpCode
  LanguagePrimitives.EnumToValue httpStatusCode = httpCode.code
  
  
  
