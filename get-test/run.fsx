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
let playlistSample = __SOURCE_DIRECTORY__ + "/Sample/playlistSample.json"

[<Literal>]
let calendarSample = __SOURCE_DIRECTORY__ + "/Sample/calendar.json"
type OfficeCalendar = JsonProvider<calendarSample>

[<Literal>]
let tokenFile = __SOURCE_DIRECTORY__ + "/tokenFile.txt"

let createResponse data =
    let response = new HttpResponseMessage()
    response.Content <- new StringContent(data)
    response.StatusCode <- HttpStatusCode.OK
    response.Content.Headers.ContentType <- MediaTypeHeaderValue("application/json")
    response

let createPlaylist name (playlists: OfficeCalendar.Root) =
    let events = playlists.Value |> Seq.filter (fun event -> event.Organizer.EmailAddress.Address = name || (event.Attendees |> Seq.exists (fun a -> a.EmailAddress.Address = name)))
                            |> Seq.map (fun event -> "{" + "UniqueId = " + event.Id + ", Content = " + event.Location.DisplayName + "}")
                            |> String.concat ","
    """{"Groups":[{"UniqueId":"audio_video_picture","Title":"Universal Media Player Tests","Category":"Windows 10 Universal Media Player Tests","ImagePath": "ms-appx:///Assets/AudioVideo.png","Description":"Windows 10 Universal Media Player Tests","Items": [""" + events + "]}]}"

let getPlaylistFromCalendar name token (log: TraceWriter) =
    let now = DateTime.Now
    let url = "https://graph.microsoft.com/v1.0/me/calendar/calendarView?startDateTime=" + now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff") + "&endDateTime=" + now.AddSeconds(1.).ToString("yyyy-MM-ddTHH:mm:ss.fffffff")
    let playlists = Http.RequestString(url, headers = [ "Authorization", "Bearer " + token])
    log.Info(sprintf "playlists: %s" playlists)
    playlists |> OfficeCalendar.Parse |> createPlaylist name
    //File.ReadAllText(playlistSample)

let getPlaylist name (log: TraceWriter) =
    match File.ReadAllText(tokenFile) with
    | "null" -> File.ReadAllText(playlistSample)
    | token -> getPlaylistFromCalendar name token log

let Run(req: HttpRequestMessage, name: string, log: TraceWriter) =
    getPlaylist name log |> createResponse
