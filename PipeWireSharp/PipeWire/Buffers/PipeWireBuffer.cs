using PipeWireSharp.Native;
using PipeWireSharp.Spa.Buffers;

namespace PipeWireSharp.PipeWire.Buffers;

public class PipeWireBuffer
{
    internal readonly IntPtr Handle;
    internal unsafe pw_buffer* RawHandle => (pw_buffer*)Handle;

    internal PipeWireBuffer(IntPtr handle)
    {
        Handle = handle;
    }

    public ulong Requested
    {
        get
        {
            unsafe
            {
                return RawHandle->requested;
            }
        }
    }
    
    public ulong Size
    {
        get
        {
            unsafe
            {
                return RawHandle->size;
            }
        }
    }
    
    public ulong Time
    {
        get
        {
            unsafe
            {
                return RawHandle->time;
            }
        }
    }
    
    public SpaBuffer Buffer
    {
        get
        {
            unsafe
            {
                return new SpaBuffer((IntPtr)RawHandle->buffer);
            }
        }
    }
}