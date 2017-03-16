#if !COMPILED
#r "../packages/FSharp.Data.dll"
#r "../packages/Microsoft.Azure.WebJobs.Host.dll"
#r "../packages/Microsoft.Azure.WebJobs.Extensions.dll"
#endif

#r "System.Net.Http"

open System
open System.Linq
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.IO
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

open Microsoft.Azure.WebJobs.Host
open Microsoft.Azure.WebJobs

[<Literal>]
let playlistSample = __SOURCE_DIRECTORY__ + "/playlistSample.json"

[<Literal>]
let calendarSample = __SOURCE_DIRECTORY__ + "/calendarSample.json"
type OfficeCalendar = JsonProvider<calendarSample>

[<Literal>]
let tokenFile = __SOURCE_DIRECTORY__ + "/tokenFile.txt"

let createResponse data =
    let response = new HttpResponseMessage()
    response.Content <- new StringContent(data)
    response.StatusCode <- HttpStatusCode.OK
    response.Content.Headers.ContentType <- MediaTypeHeaderValue("application/json")
    response

let getPlaylistFromCalendar token (log: TraceWriter) =
    let now = DateTime.Now
    let url = "https://graph.microsoft.com/v1.0/me/calendar/calendarView?startDateTime=" + now.ToString("o") + "&endDateTime=" + now.AddSeconds(1.).ToString("o")
    let playlists = Http.RequestString(url, headers = [ "Authorization", "Bearer " + token])
    log.Info(sprintf "playlists: %s" playlists)
    File.ReadAllText(playlistSample)

let getPlaylist (log: TraceWriter) =
    match File.ReadAllText(playlistSample) with
    | "null" -> File.ReadAllText(playlistSample)
    | token -> getPlaylistFromCalendar token log

let Run(req: HttpRequestMessage, log: TraceWriter) =
    getPlaylist log |> createResponse
