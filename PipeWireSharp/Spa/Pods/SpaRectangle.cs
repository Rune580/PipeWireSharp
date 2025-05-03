using PipeWireSharp.Native;

namespace PipeWireSharp.Spa.Pods;

public struct SpaRectangle
{
    public uint Width;
    public uint Height;
    
    public SpaRectangle() { }

    internal SpaRectangle(spa_rectangle rectangle)
    {
        Width = rectangle.width;
        Height = rectangle.height;
    }

    public override string ToString()
    {
        return $"{{ Width: {Width}, Height: {Height} }}";
    }
}