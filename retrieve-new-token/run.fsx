open System
open System.Configuration
open FSharp.Data

[<Literal>]
let bodySample = """{"token_type":"Bearer","scope":"Calendars.Read","expires_in":"3599","ext_expires_in":"0","expires_on":"1489416293","not_before":"1489412393","resource":"https://graph.microsoft.com","access_token":"ey_-9Nv_","refresh_token":"AQ-0_"}"""
type BodyPostTokenRequest = JsonProvider<bodySample>

let Run(myTimer: TimerInfo, log: TraceWriter) =
    let appSettings = ConfigurationManager.AppSettings 
    let url = appSettings.["TokenUrl"]
    let postInformation = "grant_type=" + appSettings.["GrantType"] +
                   "&client_id=" + appSettings.["ClientId"] +
                   "&client_secret=" + appSettings.["ClientSecret"] +
                   "&resource=" + appSettings.["RessourceUrl"] +
                   "&username=" + appSettings.["Username"] +
                   "&password=" + appSettings.["Password"] +
                   "&scope=" + appSettings.["Scope"]

    log.Info(sprintf "Message sent: %s" postData)

    let bodyResult = Http.RequestString(url, headers = [ ContentType HttpContentTypes.FormValues ], body = TextRequest postInformation)

    log.Info(sprintf "token: %s" result.AccessToken)
