module MW

open System
//open System.Collections.Generic
//open System.Diagnostics
open System.IO
open OpenTK
open OpenTK.Graphics
open OpenTK.Graphics.OpenGL4
open OpenTK.Input
open RenderObject
open Vertex

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
    let mutable _renderObjects = new System.Collections.Generic.List<RenderObject>()


    //let mutable _frames = 50L
    //let mutable _factor = 0.
    let onClosed(evenArgs: System.EventArgs) = mw.Exit()
    let HandleKeyBoard() =
        let keyState = Keyboard.GetState()
        if keyState.IsKeyDown(Key.Escape) then mw.Exit()
    do mw.Title <- _title

    let defaultShaderPath = 
        @"G:\NetWorkDrive\Dropbox\dev\F#\openTk\Tutorial\fsOpenTkTurorial\fsOpenTkTurorial\Components\Shaders\"

    let CompileShaders(shaderType: ShaderType, path: string) =
        let shader = GL.CreateShader(shaderType)
        let src = File.ReadAllText(path)
        GL.ShaderSource(shader, src)
        GL.CompileShader(shader)
        let info = GL.GetShaderInfoLog(shader)
        if info <> "" then 
            printfn "GL.CompileShader [%A] had info log: %A" shaderType info
        shader
    let CreateProgram() =
        let program = GL.CreateProgram()
        let shaders = [
            CompileShaders(ShaderType.VertexShader, 
                              //defaultShaderPath + @"1Vert\vertexShader.c")
                              defaultShaderPath + @"vertexShader.vert")
            CompileShaders(ShaderType.FragmentShader, 
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
        let vertices = [|
            new Vertex(new Vector4(-0.25f, 0.25f, 0.5f, 1.f), Color4.HotPink)
            new Vertex(new Vector4( 0.0f, -0.25f, 0.5f, 1.f), Color4.HotPink)
            new Vertex(new Vector4( 0.25f, 0.25f, 0.5f, 1.f), Color4.HotPink)
        |]
        _renderObjects.Add(new RenderObject(vertices))
        mw.CursorVisible <- true
        _program <- CreateProgram()
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)
        GL.PatchParameter(PatchParameterInt.PatchVertices, 3)
        mw.Closed.Add(onClosed)
    override mw.OnUpdateFrame(e: FrameEventArgs) =
        HandleKeyBoard() 
    override mw.OnRenderFrame(e: FrameEventArgs) =
        _time <- e.Time + _time
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
        for obj in _renderObjects do obj.Render()
        mw.SwapBuffers()
    override mw.Exit() =
        //For Debug
        printfn "mw existed"
        for (obj: RenderObject) in _renderObjects do (obj:>IDisposable).Dispose()
        GL.DeleteProgram(_program)
        base.Exit()


