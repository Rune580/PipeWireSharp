using System.Runtime.InteropServices;
using PipeWireSharp.Native;

namespace PipeWireSharp.PipeWire.Streams;

public class StreamControl
{
    internal readonly IntPtr Handle;
    internal unsafe pw_stream_control* RawHandle => (pw_stream_control*)Handle;

    public StreamControl(IntPtr handle)
    {
        Handle = handle;
    }

    public string Name
    {
        get
        {
            unsafe
            {
                return Marshal.PtrToStringAuto((IntPtr)RawHandle->name) ?? "";
            }
        }
    }

    /// <summary>
    /// Extra flags (unused)
    /// </summary>
    public uint Flags
    {
        get
        {
            unsafe
            {
                return RawHandle->flags;
            }
        }
    }

    /// <summary>
    /// Default value
    /// </summary>
    public float Default
    {
        get
        {
            unsafe
            {
                return RawHandle->def;
            }
        }
    }
    
    /// <summary>
    /// Minimum value
    /// </summary>
    public float Min
    {
        get
        {
            unsafe
            {
                return RawHandle->min;
            }
        }
    }
    
    /// <summary>
    /// Maximum value
    /// </summary>
    public float Max
    {
        get
        {
            unsafe
            {
                return RawHandle->max;
            }
        }
    }
    
    /// <summary>
    /// Array of values
    /// </summary>
    public float[] Values
    {
        get
        {
            unsafe
            {
                var valueCount = (int)RawHandle->n_values;
                var values = new float[valueCount];
                
                Marshal.Copy((IntPtr)RawHandle->values, values, 0, valueCount);

                return values;
            }
        }
    }
    
    /// <summary>
    /// Number of values in array 
    /// </summary>
    public uint NumberOfValues
    {
        get
        {
            unsafe
            {
                return RawHandle->n_values;
            }
        }
    }
    
    /// <summary>
    /// Max values that can be set on this control 
    /// </summary>
    public uint MaxValues
    {
        get
        {
            unsafe
            {
                return RawHandle->max_values;
            }
        }
    }
}