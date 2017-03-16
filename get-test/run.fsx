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
let playlistSample = __SOURCE_DIRECTORY__ + "/playlist.json"

[<Literal>]
let CalendarSample = """{
  "@odata.context": "https://graph.microsoft.com/v1.0/$metadata#users('bbe1')/calendar/calendarView",
  "value": [
    {
      "@odata.etag": "W/\"CTRAAAAe0Q==\"",
      "id": "AAMkAABzCAAA=",
      "createdDateTime": "2017-03-13T09:55:37.4440925Z",
      "lastModifiedDateTime": "2017-03-13T09:55:37.6628482Z",
      "changeKey": "CTRo1luAAAAAe0Q==",
      "categories": [],
      "originalStartTimeZone": "Pacific Standard Time",
      "originalEndTimeZone": "Pacific Standard Time",
      "iCalUId": "0400087C",
      "reminderMinutesBeforeStart": 15,
      "isReminderOn": true,
      "hasAttachments": false,
      "subject": "Test video and invited other account",
      "bodyPreview": "",
      "importance": "normal",
      "sensitivity": "normal",
      "isAllDay": false,
      "isCancelled": false,
      "isOrganizer": true,
      "responseRequested": true,
      "seriesMasterId": null,
      "showAs": "busy",
      "type": "singleInstance",
      "webLink": "https://outlook.office365.com",
      "onlineMeetingUrl": null,
      "responseStatus": {
        "response": "organizer",
        "time": "0001-01-01T00:00:00Z"
      },
      "body": {
        "contentType": "html",
        "content": "<html>\r\n<head>\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\r\n<meta content=\"text/html; charset=us-ascii\">\r\n<style type=\"text/css\" style=\"display:none\">\r\n<!--\r\np\r\n\t{margin-top:0;\r\n\tmargin-bottom:0}\r\n-->\r\n</style>\r\n</head>\r\n<body dir=\"ltr\"></body>\r\n</html>\r\n"
      },
      "start": {
        "dateTime": "2017-03-14T10:00:00.0000000",
        "timeZone": "UTC"
      },
      "end": {
        "dateTime": "2017-03-14T12:00:00.0000000",
        "timeZone": "UTC"
      },
      "location": {
        "displayName": "http://clips.vorwaerts-gmbh.de/VfE_html5.mp4"
      },
      "recurrence": null,
      "attendees": [
        {
          "type": "required",
          "status": {
            "response": "none",
            "time": "0001-01-01T00:00:00Z"
          },
          "emailAddress": {
            "name": "EcranTv",
            "address": "lala@xxx.fr"
          }
        }
      ],
      "organizer": {
        "emailAddress": {
          "name": "test",
          "address": "lala@xxx.fr"
        }
      }
    }
  ]
}"""
type OfficeCalendar = JsonProvider<CalendarSample>

[<Literal>]
let tokenFile = __SOURCE_DIRECTORY__ + "/tokenFile.txt"

let createResponse data =
    let response = new HttpResponseMessage()
    response.Content <- new StringContent(data)
    response.StatusCode <- HttpStatusCode.OK
    response.Content.Headers.ContentType <- MediaTypeHeaderValue("application/json")
    response

let createPlaylist name playlists =
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

let getPlaylist name (log: TraceWriter) =
    match File.ReadAllText(tokenFile) with
    | "null" -> File.ReadAllText(playlistSample)
    | token -> getPlaylistFromCalendar name token log

let Run(req: HttpRequestMessage, name: string, log: TraceWriter) =
    getPlaylist name log |> createResponse
