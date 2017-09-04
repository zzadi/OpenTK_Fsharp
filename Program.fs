(* For FSI
#I @"Util"
#load @"Util.fs"
"Part04" |> Util.backUpToFolder

#r @"G:\NetWorkDrive\Dropbox\dev\F#\openTk\Tutorial\fsOpenTkTurorial\packages\OpenTK.2.0.0\lib\net20\OpenTK.dll"
#r @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Drawing.dll"

#load @"MainWindow.fs"
let mw = new MW.MainWindow()
mw.Run(60.0)


*)

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let mw = new MW.MainWindow()
    mw.Run(60.0)
    0 // return an integer exit code
