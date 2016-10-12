module Suave.Azure.Functions.Tests.Context

open Xunit
open xunit.jet.Assert
open Suave
open Suave.Successful
open Suave.Operators
open Suave.Filters
open Suave.Writers
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open Suave.Azure.Functions.Context
open TestUtil

let url = "https://localhost:8083/api/hello"

[<Fact>]
let ``runWebPart runs the WebPart from the POST HttpRequestMessage``() =
  let req = new HttpRequestMessage(HttpMethod.Post, url)
  let app = 
    POST >=> OK "hello" >=> 
      setHeader "X-Powered-By" "Suave.Azure.Functions" >=>
      setHeader "Content-Type" "application/json"
  let res = runWebPart app req |> Async.RunSynchronously
  
  let content = runTask <| res.Content.ReadAsStringAsync()
  equalDeep "hello" content
  equalDeep "Suave.Azure.Functions" (res.Content.Headers.GetValues "X-Powered-By" |> Seq.head)
  equalDeep HttpStatusCode.OK res.StatusCode

[<Fact>]
let ``runWebPart returns NotFound if WebPart execution returns none``() =
  let req = new HttpRequestMessage(HttpMethod.Post, url)
  let app = GET >=> OK "hello"
  let res = runWebPart app req |> Async.RunSynchronously

  equalDeep HttpStatusCode.NotFound res.StatusCode

[<Fact>]
let ``runWebPart takes path from request header if present``() =
  let req = new HttpRequestMessage(HttpMethod.Post, "https://foobar.azurewebsites.net/api/HelloRest")
  req.Headers.Add("X-Suave-Path", "/api/Hello")
  let app = path "/api/Hello" >=> OK "hello"  
  let res = runWebPart app req |> Async.RunSynchronously

  let content = runTask <| res.Content.ReadAsStringAsync()
  equalDeep "hello" content
  equalDeep HttpStatusCode.OK res.StatusCode

[<Fact>]
let ``runWebPart returns NotFound if WebPart execution returns none for custom path``() =
  let req = new HttpRequestMessage(HttpMethod.Post, url)
  let app = path "/api/Persons" >=> OK "hello"
  let res = runWebPart app req |> Async.RunSynchronously

  equalDeep HttpStatusCode.NotFound res.StatusCode

