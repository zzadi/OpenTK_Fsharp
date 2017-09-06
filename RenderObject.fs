module RenderObject
open OpenTK.Graphics.OpenGL4
open System
open Vertex

type RenderObject (vertices: Vertex.Vertex []) =
    let _verticeCount = vertices.Length
    let _vertexArray = GL.GenVertexArray()
    let _buffer = GL.GenBuffer()
    let mutable _initialized = true
    let Dispose(disposing) =
        if disposing && _initialized then
            GL.DeleteVertexArray(_vertexArray)
            GL.DeleteBuffer(_buffer)
            _initialized <- false

    do GL.BindVertexArray(_vertexArray)
    do GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexArray)

    // create first buffer: vertex
    do GL.NamedBufferStorage(
        _buffer,
        Vertex.Size * vertices.Length,        // the size needed by this buffer
        vertices,                           // data to initialize with
        BufferStorageFlags.MapWriteBit)    // at this point we will only write to the buffer


    do GL.VertexArrayAttribBinding(_vertexArray, 0, 0)
    do GL.EnableVertexArrayAttrib(_vertexArray, 0)
    do GL.VertexArrayAttribFormat(
        _vertexArray,
        0,                      // attribute index, from the shader location = 0
        4,                      // size of attribute, vec4
        VertexAttribType.Float, // contains floats
        false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
        0)                     // relative offset, first item


    do GL.VertexArrayAttribBinding(_vertexArray, 1, 0)
    do GL.EnableVertexArrayAttrib(_vertexArray, 1)
    do GL.VertexArrayAttribFormat(
        _vertexArray,
        1,                      // attribute index, from the shader location = 1
        4,                      // size of attribute, vec4
        VertexAttribType.Float, // contains floats
        false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
        16)                     // relative offset after a vec4

    // link the vertex array and buffer and provide the stride as size of Vertex
    do GL.VertexArrayVertexBuffer(_vertexArray, 0, _buffer, IntPtr.Zero, Vertex.Size)
    member this.Render() =
        GL.BindVertexArray(_vertexArray)
        GL.DrawArrays(PrimitiveType.Triangles, 0, _verticeCount)
    interface IDisposable with 
        member this.Dispose() = 
            Dispose(true)
            GC.SuppressFinalize(this)
   