using System.Diagnostics;

namespace PipeWireSharp.BindGen.Tools;

internal static class Cargo
{
    public static void Build(string projectDir, string configuration = "release")
    {
        using var cargoProcess = new Process();
        cargoProcess.StartInfo = new ProcessStartInfo
        {
            FileName = "cargo",
            Arguments = $"build -vv --{configuration}",
            UseShellExecute = false,
            WorkingDirectory = projectDir
        };

        cargoProcess.Start();
        cargoProcess.WaitForExit();
    }
}