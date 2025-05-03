using PipeWireSharp.Native;

namespace PipeWireSharp.PipeWire;

public class MainLoop
{
    internal readonly IntPtr Handle;
    internal unsafe pw_main_loop* RawHandle => (pw_main_loop*)Handle;

    public MainLoop()
    {
        unsafe
        {
            var props = (spa_dict*)IntPtr.Zero;
            var ptr = Bindings.pw_main_loop_new(null);
            Handle = (IntPtr)ptr;
        }
    }

    internal unsafe pw_loop* GetLoop()
    {
        return Bindings.pw_main_loop_get_loop(RawHandle);
    }

    public void Run()
    {
        unsafe
        {
            var result = Bindings.pw_main_loop_run(RawHandle);

            if (result < 0)
                Console.WriteLine($"Got Error while running MainLoop: {result}!"); // TODO Logging. 
        }
    }
}