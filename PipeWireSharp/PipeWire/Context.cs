using System.Runtime.InteropServices;
using PipeWireSharp.Native;

namespace PipeWireSharp.PipeWire;

public class Context
{
    internal readonly IntPtr Handle;
    internal unsafe pw_context* RawHandle => (pw_context*)Handle;
    
    public readonly MainLoop Loop;

    public Context(MainLoop mainLoop)
    {
        Loop = mainLoop;

        unsafe
        {
            var props = (pw_properties*)IntPtr.Zero;

            Handle = (IntPtr)Bindings.pw_context_new(Loop.GetLoop(), null, 0);
        }
    }

    public Core ConnectFd(SafeHandle fileHandle, PwProperties? properties = null)
    {
        unsafe
        {
            var coreHandle = Bindings.pw_context_connect_fd(RawHandle, fileHandle.DangerousGetHandle().ToInt32(), null, 0);
            return new Core((IntPtr)coreHandle);
        }
    }
}