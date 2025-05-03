using PipeWireSharp.BindGen.Tools;

namespace PipeWireSharp.BindGen;

internal static class Program
{
    private const string PipeWireGit = "https://gitlab.freedesktop.org/pipewire/pipewire-rs.git";
    private const string PipeWireReleaseTag = "v0.8.0";

    public static void Main(string[] args)
    {
        var workDir = Path.Join(Environment.CurrentDirectory, "workDir");

        if (Directory.Exists(workDir))
            Directory.Delete(workDir, true);
        
        var repoDir = Git.CloneRepository(PipeWireGit, workDir);
        Git.CheckoutTag(repoDir, PipeWireReleaseTag);
        
        var targetDir = Path.Join(PipeWireBindGenPath, "src");
        
        // Copy necessary files to pipewire-bindgen
        File.Copy(Path.Join(repoDir, "libspa-sys/src/type-info.c"), Path.Join(targetDir, "type-info.c"), true);
        File.Copy(Path.Join(repoDir, "libspa-sys/wrapper.h"), Path.Join(PipeWireBindGenPath, "spa-wrapper.h"), true);
        File.Copy(Path.Join(repoDir, "pipewire-sys/wrapper.h"), Path.Join(PipeWireBindGenPath, "pipewire-wrapper.h"), true);
        
        // Generate Bindings
        Cargo.Build(PipeWireBindGenPath);
        
        // Post Process
        PostProcess.Run(Path.Join(Environment.CurrentDirectory, "../../../../PipeWireSharp/Native/Bindings.g.cs"));
        
        if (Directory.Exists(workDir))
            Directory.Delete(workDir, true);
    }
    
    private static string PipeWireBindGenPath => Path.Join(Environment.CurrentDirectory, "../../../../pipewire-bindgen");
}