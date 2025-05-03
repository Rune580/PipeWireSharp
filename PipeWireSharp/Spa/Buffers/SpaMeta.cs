using PipeWireSharp.Native;

namespace PipeWireSharp.Spa.Buffers;

public class SpaMeta
{
    internal readonly IntPtr Handle;
    internal unsafe spa_meta* RawHandle => (spa_meta*)Handle;

    internal SpaMeta(IntPtr handle)
    {
        Handle = handle;
    }
}