using System.Runtime.InteropServices;
using PipeWireSharp.Native;
using PipeWireSharp.PipeWire.Buffers;
using PipeWireSharp.Spa.Pods;
using PipeWireSharp.Spa.Utils;
using PipeWireSharp.Utils;

namespace PipeWireSharp.PipeWire.Streams;

public class Stream
{
    private readonly Core _core;
    
    internal readonly IntPtr Handle;
    internal unsafe pw_stream* RawHandle => (pw_stream*)Handle;

    public Stream(Core core, string name, PwProperties properties)
    {
        _core = core;
        
        unsafe
        {
            var namePtr = name.AllocUtf8Ptr();

            Handle = (IntPtr)Bindings.pw_stream_new(core.RawHandle, (byte*)namePtr, properties.RawHandle);
            
            Marshal.FreeHGlobal(namePtr);
        }
    }

    public void Connect(Direction direction, uint nodeId, StreamFlags flags, params IPodSerializable[] paramPods)
    {
        unsafe
        {
            var pods = paramPods.Select(serializable => serializable.ToPod())
                .Select(pod => pod.Handle).ToArray();

            fixed (IntPtr* podsPtr = pods)
            {
                var podsArray = (spa_pod**)podsPtr;

                int result;

                if (paramPods.Length > 0)
                {
                    result = Bindings.pw_stream_connect(RawHandle, (uint)direction, nodeId, (uint)flags, podsArray, (uint)pods.Length);
                }
                else
                {
                    result = Bindings.pw_stream_connect(RawHandle, (uint)direction, nodeId, (uint)flags, (spa_pod**)IntPtr.Zero, 0);
                }

                if (result < 0)
                    throw new Exception($"Failed to connect stream! Error: {result}");
            }
        }
    }

    public StreamListenerBuilder AddListener() => new(this);

    public void UpdateParams(params IPodSerializable[] paramPods)
    {
        unsafe
        {
            var pods = paramPods.Select(serializable => serializable.ToPod())
                .Select(pod => pod.Handle).ToArray();

            fixed (IntPtr* podsPtr = pods)
            {
                var podsArray = (spa_pod**)podsPtr;
                var result = Bindings.pw_stream_update_params(RawHandle, podsArray, (uint)pods.Length);

                if (result < 0)
                    throw new Exception($"Failed to update params! Error: {result}");
            }
        }
    }

    public PipeWireBuffer DequeueBuffer()
    {
        unsafe
        {
            var buffer = Bindings.pw_stream_dequeue_buffer(RawHandle);
            
            return new PipeWireBuffer((IntPtr)buffer);
        }
    }
    
    public void QueueBuffer(PipeWireBuffer buffer)
    {
        unsafe
        {
            var result = Bindings.pw_stream_queue_buffer(RawHandle, (pw_buffer*)buffer.Handle);
            if (result < 0)
                throw new Exception($"Failed to queue buffer! Error: {result}");
        }
    }
}