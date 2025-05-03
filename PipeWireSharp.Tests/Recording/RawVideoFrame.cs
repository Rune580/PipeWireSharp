using FFMpegCore.Pipes;

namespace PipeWireSharp.Tests.Recording;

public class RawVideoFrame : IVideoFrame
{
    private readonly Memory<byte> _data;
    public int Width { get; }
    public int Height { get; }
    public string Format { get; }

    internal RawVideoFrame(Memory<byte> data, int width, int height, string format)
    {
        _data = data;
        Width = width;
        Height = height;
        Format = format;
    }
    
    public void Serialize(Stream pipe)
    {
        pipe.Write(_data.Span);
    }

    public Task SerializeAsync(Stream pipe, CancellationToken token)
    {
        var task = pipe.WriteAsync(_data, token);
        return task.AsTask();
    }
}