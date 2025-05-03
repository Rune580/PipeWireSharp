using System.Reflection;
using System.Runtime.InteropServices;
using PipeWireSharp.Native;

namespace PipeWireSharp;

public static class PipeWireSharpLib
{
    public static void Init()
    {
        Bindings.pw_init();
        
        // NativeLibrary.Load(FindLibrary("libspa.so"));
        // NativeLibrary.Load("libpipewire-0.3.so");
    }

    private static string FindLibrary(string pattern)
    {
        foreach (var file in Directory.EnumerateFiles("/usr/lib", pattern, SearchOption.AllDirectories))
        {
            return file;
        }

        throw new FileNotFoundException("Could not find Library!");
    }
}