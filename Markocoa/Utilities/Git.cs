
using System.Diagnostics;

namespace Markocoa.Utilities;

/// <summary>
/// Utility class for handling Git operations.
/// </summary>
internal static class Git
{
    /// <summary>
    /// Executes a Git command.
    /// </summary>
    /// <param name="command">Command to execute.</param>
    public static void Execute(string command, string workingDirectory)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = command,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();
    }
}
