// Learn more about F# at http://fsharp.org

open System
open FSharp.Compiler.SourceCodeServices
open Dotnet.ProjInfo.Workspace
open Dotnet.ProjInfo.Workspace.FCS
open System.IO

let parseAndTypeCheck projPath =

    let msbuildLocator = MSBuildLocator()
    let loader =
        LoaderConfig.Default msbuildLocator
        |> Loader.Create

    let netFwInfo =
        NetFWInfoConfig.Default msbuildLocator
        |> NetFWInfo.Create

    let fcs =

      let checker =
        FSharpChecker.Create(
          projectCacheSize = 200,
          keepAllBackgroundResolutions = true,
          keepAssemblyContents = true)

      checker.ImplicitlyStartBackgroundWork <- true

      checker

    let fcsBinder = FCSBinder(netFwInfo, loader, fcs)

    printfn "Loading %s" projPath

    loader.Notifications.Subscribe(fun (l, ws) -> printfn "%A" ws) |> ignore

    loader.LoadProjects [ projPath ]

    match fcsBinder.GetProjectOptions(projPath) with
    | None ->
        printfn "Cannot get FCS project options for %s" projPath
    | Some fcsPo ->
        printfn "FcsPo: %A" fcsPo

        printfn "running ParseAndCheckProject ..."

        let result =
          fcs.ParseAndCheckProject(fcsPo)
          |> Async.RunSynchronously

        if result.Errors.Length > 0 then
            printfn "FCS Result errors: %A" result.Errors
        else
            printfn "running GetAllUsesOfAllSymbols ..."

            let uses =
              result.GetAllUsesOfAllSymbols()
              |> Async.RunSynchronously

            printfn "Usages: %A" uses


[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"

    let projPath = Path.GetFullPath "repro/reproducer.fsproj"

    parseAndTypeCheck projPath

    0 // return an integer exit code
