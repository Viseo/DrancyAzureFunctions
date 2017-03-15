#if !COMPILED
#r "../packages/Microsoft.Azure.WebJobs.Host.dll"
#endif

#r "System.Net.Http"

open System.Linq
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.IO

open Microsoft.Azure.WebJobs.Host

[<Literal>]
let playlistSample = __SOURCE_DIRECTORY__ + "/playlistSample.json"

let createResponse data =
    let response = new HttpResponseMessage()
    response.Content <- new StringContent(data)
    response.StatusCode <- HttpStatusCode.OK
    response.Content.Headers.ContentType <- MediaTypeHeaderValue("application/json")
    response

let Run(req: HttpRequestMessage, log: TraceWriter) =
    File.ReadAllText(playlistSample) |> createResponse
