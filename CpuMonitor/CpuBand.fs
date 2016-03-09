namespace CpuMonitor

open System
open Newtonsoft.Json

type ThresholdState =
    | Under
    | OverSince of since: DateTime

type CpuBand (threshold:float32, state, totalTimeOver) =
    let mutable state = state
    let mutable totalTimeOver = totalTimeOver

    new (threshold) =
        new CpuBand(threshold, Under, TimeSpan.Zero)

    member this.Threshold = threshold
    member this.State with get() = state
    member this.TotalTimeOver with get() = totalTimeOver
    [<JsonIgnore>]
    member val Clock = fun() -> DateTime.Now with get, set // Settable for testing purposes        

    member this.Update cpuValue =
        let now = this.Clock()
        if cpuValue < threshold then
            match state with
            | Under -> ()
            | OverSince time -> 
                let timeOver = now - time
                state <- Under
                totalTimeOver <- totalTimeOver + timeOver
        else
            match state with
            | Under -> state <- OverSince now
            | OverSince _ -> ()

