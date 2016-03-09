namespace CpuMonitor
open System
open System.Diagnostics
open System.IO

type Monitor(thresholds) =
    let counter = new PerformanceCounter("Processor", "% Processor Time", "_Total")
    let bands = thresholds |> Seq.map(fun t -> new CpuBand(float32 t)) |> List.ofSeq

    member this.Update() =
        let cpuValue = counter.NextValue()
        bands |> List.iter (fun b -> b.Update cpuValue)

    member this.Save(filePath) =
        Persistence.saveAsJson filePath bands        

    interface IDisposable with
        member this.Dispose() = counter.Dispose()
