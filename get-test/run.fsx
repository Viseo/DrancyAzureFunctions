#r "System.Net.Http"

open System.Linq
open System.Net
open System.Net.Http
open System.IO

[<Literal>]
let sampleRate = __SOURCE_DIRECTORY__ + "/playlistSample.json"

let Run(req: HttpRequestMessage, log: TraceWriter) =
    let content = File.ReadAllText(sampleRate)
    req.CreateResponse(HttpStatusCode.OK, content)
