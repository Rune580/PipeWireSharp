namespace PipeWireSharp.BindGen.Tools;

internal static class PostProcess
{
    public static void Run(string bindingsFile)
    {
        var fileContents = File.ReadAllText(bindingsFile);
        
        fileContents = FixBindGenBitfieldUnit(fileContents);
        fileContents = FixPwInit(fileContents);
        
        File.WriteAllText(bindingsFile, fileContents);
    }

    private static string FixBindGenBitfieldUnit(string fileContents)
    {
        Console.WriteLine("Fixing BindGenBitField unit");
        
        fileContents = fileContents.Replace("public Storage storage;", "public fixed byte storage[3];");
        
        return fileContents;
    }

    private static string FixPwInit(string fileContents)
    {
        Console.WriteLine("Fixing pw_init");
        
        fileContents = fileContents.Replace(
            """[DllImport(__DllName, EntryPoint = "csbindgen_pw_deinit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]""",
            "[DllImport(__DllName, EntryPoint = \"csbindgen_pw_init\", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]\n" +
            "\t\tinternal static extern void pw_init();\n\n" +
            "\t\t[DllImport(__DllName, EntryPoint = \"csbindgen_pw_deinit\", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]"
        );

        return fileContents;
    }
}