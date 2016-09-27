module Suave.Azure.Functions.Response
  
open System.Net
open Suave.Http

let httpStatusCode (httpCode : HttpCode) : HttpStatusCode = 
  LanguagePrimitives.EnumOfValue httpCode.code