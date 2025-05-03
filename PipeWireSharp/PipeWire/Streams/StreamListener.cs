using System.Runtime.InteropServices;

namespace PipeWireSharp.PipeWire.Streams;

public class StreamListener : IDisposable
{
    private readonly IntPtr _hooks;
    private readonly IntPtr _streamEvents;

    private readonly object[] _delegates;

    internal StreamListener(IntPtr hooks, IntPtr events, params object[] delegates)
    {
        _hooks = hooks;
        _streamEvents = events;
        
        _delegates = delegates;
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(_hooks);
        Marshal.FreeHGlobal(_streamEvents);
        
        Array.Clear(_delegates);
    }
}