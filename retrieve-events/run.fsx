
#if !COMPILED
#r "../packages/Microsoft.Azure.WebJobs.Host.dll"
#endif

open Microsoft.Azure.WebJobs.Host

let Run(token: string, log: TraceWriter) =
    log.Info(sprintf "F# Queue trigger function processed: '%s'" token)
