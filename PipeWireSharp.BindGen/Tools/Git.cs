using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PipeWireSharp.BindGen.Tools;

internal static partial class Git
{
    [GeneratedRegex(@"https:\/\/.*\/(?<repo_name>.*)\.git$", RegexOptions.Multiline | RegexOptions.Compiled)]
    private static partial Regex RepoNameRegex();
    
    public static string CloneRepository(string url, string workDir)
    {
        if (!Directory.Exists(workDir))
            Directory.CreateDirectory(workDir);
        
        var repoName = RepoNameRegex().Match(url).Groups["repo_name"].Value;
        
        using var gitProcess = new Process();
        gitProcess.StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"clone {url}",
            UseShellExecute = false,
            WorkingDirectory = workDir
        };
        
        Console.WriteLine("Cloning pipewire-rs");
        
        gitProcess.Start();
        gitProcess.WaitForExit();
        
        return Path.Join(workDir, repoName);
    }

    public static void CheckoutTag(string repoDir, string tagName)
    {
        if (!Directory.Exists(repoDir))
            throw new DirectoryNotFoundException();
        
        using var gitProcess = new Process();
        gitProcess.StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"checkout \"tags/{tagName}\"",
            UseShellExecute = false,
            WorkingDirectory = repoDir
        };
        
        gitProcess.Start();
        gitProcess.WaitForExit();
    }
}