open System
open System.Configuration
open System.Threading
open CpuMonitor

let update (monitor:Monitor) state =
    printfn "[%A] Updating timers" DateTime.Now
    monitor.Update()

let save (monitor:Monitor) filepath state =
    printfn "[%A] Saving" DateTime.Now
    monitor.Save filepath

[<EntryPoint>]
let main argv = 
    let thresholds = ConfigurationManager.AppSettings.["Thresholds"].Split ',' |> Array.map int
    let filePath = ConfigurationManager.AppSettings.["Persistence.Filepath"]
    Persistence.archive filePath
    use monitor = new Monitor(thresholds)
    use updateTimer = new Timer(update monitor, null, TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
    use saveTimer = new Timer(save monitor filePath, null, TimeSpan.FromSeconds(2.0), TimeSpan.FromMinutes(1.0))

    while Console.ReadKey().Key <> ConsoleKey.Q do ()
    update monitor ()
    save monitor filePath ()
    0 // return an integer exit code
