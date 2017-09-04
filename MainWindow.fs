module MW

open System
//open System.Collections.Generic
//open System.Diagnostics
open System.IO
open OpenTK
open OpenTK.Graphics
open OpenTK.Graphics.OpenGL4
open OpenTK.Input

type MainWindow () as mw =
    inherit GameWindow(750,
                       500,
                       GraphicsMode.Default,
                       "Az MW",
                       GameWindowFlags.Default,
                       DisplayDevice.Default,
                       4,
                       5,
                       GraphicsContextFlags.ForwardCompatible)
    let _title = 
        sprintf "%s | OpenGL Version: %s" mw.Title (GL.GetString(StringName.Version))
    let mutable _program = 0
    let mutable _vertexArray = 0
    let mutable _time = 0.
    let mutable _frames = 50L
    let mutable _factor = 0.
    let onClosed(evenArgs: System.EventArgs) = mw.Exit()
    let HandleKeyBoard() =
        let keyState = Keyboard.GetState()
        if keyState.IsKeyDown(Key.Escape) then mw.Exit()
    do mw.Title <- _title

    let defaultShaderPath = 
        @"G:\NetWorkDrive\Dropbox\dev\F#\openTk\Tutorial\fsOpenTkTurorial\fsOpenTkTurorial\Components\Shaders\"

    member private mw.CompileShaders(shaderType: ShaderType, path: string) =
        let shader = GL.CreateShader(shaderType)
        let src = File.ReadAllText(path)
        GL.ShaderSource(shader, src)
        GL.CompileShader(shader)
        let info = GL.GetShaderInfoLog(shader)
        if info <> "" then 
            printfn "GL.CompileShader [%A] had info log: %A" shaderType info
        shader
    member private mw.CreateProgram() =
        let program = GL.CreateProgram()
        let shaders = [
            mw.CompileShaders(ShaderType.VertexShader, 
                              //defaultShaderPath + @"1Vert\vertexShader.c")
                              defaultShaderPath + @"vertexShader.vert")
            mw.CompileShaders(ShaderType.FragmentShader, 
                              //defaultShaderPath + @"5Frag\fragmentShader.c")
                              defaultShaderPath + @"fragmentShader.vert")
        ]
        shaders |> List.iter (fun shader -> GL.AttachShader(program, shader))
        GL.LinkProgram(program)
        let info = GL.GetProgramInfoLog(program)
        if info <> "" then 
            printfn "CompileShaders ProgramLinking had errors: %A" info
        shaders |> List.iter (fun shader ->
                                GL.DetachShader(program, shader)
                                GL.DeleteShader(shader))
        program      
    
        

    override mw.OnResize(e: System.EventArgs) =
        GL.Viewport(0, 0, mw.Width, mw.Height)
    override mw.OnLoad(e: System.EventArgs) =
        mw.CursorVisible <- true
        _program <- mw.CreateProgram()
        _vertexArray <- GL.GenVertexArrays(1)
        GL.BindVertexArray(_vertexArray)
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line)
        GL.PatchParameter(PatchParameterInt.PatchVertices, 3)
        mw.Closed.Add(onClosed)
    override mw.OnUpdateFrame(e: FrameEventArgs) =
        HandleKeyBoard() 
    override mw.OnRenderFrame(e: FrameEventArgs) =
        _time <- e.Time + _time
        //let frame60 = _frames % 60L
        if _frames = 60L then
            _frames <- 0L
            if _factor > 100. then
                _factor <- 0.
            else
                _factor <- _factor + 25.
        else
            _frames <- 1L + _frames
        //printfn "_time: %A" _time
        mw.Title <- sprintf "%s | (Vsync: %A) FPS: %.0f" _title mw.VSync (1. / e.Time)
        let mutable backColor = new Color4()
        backColor.A <- 1.f
        backColor.R <- 0.2f
        backColor.G <- 0.2f
        backColor.B <- 0.4f
        GL.ClearColor(backColor)
        GL.Clear(ClearBufferMask.ColorBufferBit 
                 ||| ClearBufferMask.DepthBufferBit)
        GL.UseProgram(_program)
        // add shader attributes here
        //GL.VertexAttrib1(0, _time)
        GL.VertexAttrib1(0, _factor / 100.)

        let mutable position = new Vector4()
        position.X <- System.Math.Sin(_time) * 0.5 |> float32
        //printfn "System.Math.Sin(_time) * 0.5 |> float32: %A" (System.Math.Sin(_time) * 0.5 |> float32)
        position.Y <- System.Math.Cos(_time) * 0.5 |> float32
        position.Z <- 0.f
        position.W <- 1.f
        GL.VertexAttrib4(1, position)
        // add shader attributes end
        GL.DrawArrays(PrimitiveType.Patches, 0, 3)
        GL.PointSize(10.f)
        mw.SwapBuffers()
    override mw.Exit() =
        //For Debug
        printfn "mw existed"
        GL.DeleteVertexArrays(1, ref _vertexArray)
        GL.DeleteProgram(_program)
        base.Exit()


