using PipeWireSharp.Native;

namespace PipeWireSharp.PipeWire;

public class Core
{
    internal readonly IntPtr Handle;
    internal unsafe pw_core* RawHandle => (pw_core*)Handle;

    internal Core(IntPtr handle)
    {
        Handle = handle;
    }
}