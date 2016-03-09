#load "load-project-debug.fsx"
open System
open System.Diagnostics
open CpuMonitor

let counter = new Monitor([60; 80; 90; 95; 99])


