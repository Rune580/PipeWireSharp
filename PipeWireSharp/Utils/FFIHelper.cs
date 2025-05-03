using System.Runtime.InteropServices;
using System.Text;

namespace PipeWireSharp.Utils;

internal static class FFIHelper
{
    public static IntPtr AllocUtf8Ptr(this string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);

        var ptr = Marshal.AllocHGlobal(bytes.Length);
        
        Marshal.Copy(bytes, 0, ptr, bytes.Length);

        return ptr;
    }
}