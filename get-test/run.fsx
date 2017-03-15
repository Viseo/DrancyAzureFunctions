#r "System.Net.Http"

open System.Linq
open System.Net
open System.Net.Http


let Run(req: HttpRequestMessage, log: TraceWriter) =
    let people = "Me !!!"

    req.CreateResponse(HttpStatusCode.OK, sprintf "{%s}" people)
