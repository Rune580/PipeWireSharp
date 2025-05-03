using PipeWireSharp.Native;
using PipeWireSharp.Spa.Enums;
using PipeWireSharp.Spa.Utils;

namespace PipeWireSharp.Spa.Buffers;

public class SpaData
{
    internal readonly IntPtr Handle;
    internal unsafe spa_data* RawHandle => (spa_data*)Handle;

    internal SpaData(IntPtr handle)
    {
        Handle = handle;
    }

    public uint Size
    {
        get
        {
            unsafe
            {
                return RawHandle->chunk->size;
            }
        }
    }
    
    public int Stride
    {
        get
        {
            unsafe
            {
                return RawHandle->chunk->stride;
            }
        }
    }

    public IntPtr Data
    {
        get
        {
            unsafe
            {
                return (IntPtr)((IntPtr)RawHandle->data + RawHandle->chunk->offset);
            }
        }
    }

    public ReadOnlySpan<byte> GetFrameData()
    {
        unsafe
        {
            var dataPtr = (IntPtr)RawHandle->data + RawHandle->chunk->offset;
            return new ReadOnlySpan<byte>((void*)dataPtr, (int)RawHandle->chunk->size);
        }
    }

    public VideoFrameDataAccessor AccessVideoFrameData(SpaVideoFormat videoFormat, int width, int height) =>
        new(GetFrameData(), videoFormat, Stride, width, height);
}