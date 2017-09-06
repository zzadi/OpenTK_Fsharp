module Vertex

type Vertex = struct
    val _position: OpenTK.Vector4
    val _color: OpenTK.Graphics.Color4
    static member Size = 32 //(4 + 4) * 4
    new (position, color) = 
        {_position = position;
        _color = color}
    end