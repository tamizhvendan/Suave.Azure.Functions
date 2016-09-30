module Suave.Azure.Functions.Tests

open Suave.Azure.Functions
open Suave.Http
open System.Net
open FsCheck
open FsCheck.Xunit

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
