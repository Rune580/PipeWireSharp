using System.Runtime.InteropServices;
using PipeWireSharp.Native;
using PipeWireSharp.Utils;

namespace PipeWireSharp.PipeWire;

public class PwProperties
{
    internal readonly IntPtr Handle;
    internal unsafe pw_properties* RawHandle => (pw_properties*)Handle;

    public PwProperties()
    {
        unsafe
        {
            Handle = (IntPtr)Bindings.pw_properties_new((byte*)IntPtr.Zero);
        }
    }

    public PwProperties(Dictionary<string, string> props) : this()
    {
        foreach (var (key, value) in props)
            Insert(key, value);
    }

    public void Insert(string key, string value)
    {
        unsafe
        {
            var keyPtr = key.AllocUtf8Ptr();
            var valuePtr = value.AllocUtf8Ptr();

            var result = Bindings.pw_properties_set(RawHandle, (byte*)keyPtr, (byte*)valuePtr);

            if (result < 0)
                Console.WriteLine($"Error while inserting kvp into properties: {result}! Failed to set {key} to {value}!"); // TODO Logging.
            
            Marshal.FreeHGlobal(keyPtr);
            Marshal.FreeHGlobal(valuePtr);
        }
    }
}