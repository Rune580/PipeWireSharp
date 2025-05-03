using System.Runtime.InteropServices;
using PipeWireSharp.Native;

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

    public byte[] GetFrameData()
    {
        unsafe
        {
            var dataPtr = (IntPtr)RawHandle->data + RawHandle->chunk->offset;

            var buffer = new byte[RawHandle->chunk->size];
            
            Marshal.Copy((IntPtr)dataPtr, buffer, 0, buffer.Length);
            
            return buffer;
        }
    }

    public IEnumerable<byte[]> EnumerateRows()
    {
        var bytesRead = 0;

        while (bytesRead < Size)
        {
            var buffer = new byte[Stride];
            
            unsafe
            {
                var dataPtr = (IntPtr)RawHandle->data + RawHandle->chunk->offset + bytesRead;
                Marshal.Copy((IntPtr)dataPtr, buffer, 0, buffer.Length);
                bytesRead += Stride;
            }
            
            yield return buffer;
        }
    }
}