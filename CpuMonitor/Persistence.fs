module CpuMonitor.Persistence
open System
open System.IO
open Newtonsoft.Json

type FileCreateOptions = Overwrite | Archive

let archive path =
    let directory = Path.GetDirectoryName path
    let fnwe = Path.GetFileNameWithoutExtension path
    let extension = Path.GetExtension path
    let archived = Path.Combine(directory, Path.ChangeExtension(fnwe + Guid.NewGuid().ToString(), extension))
    File.Move(path, archived)

let ensureDirectory path =
    let dir = Path.GetDirectoryName path
    if not (Directory.Exists dir) then
        Directory.CreateDirectory dir |> ignore

let saveAsJson path x =
    ensureDirectory path
    let json = JsonConvert.SerializeObject x
    File.WriteAllText(path, json)

