using System.Diagnostics;

namespace Scheduler;

public static class ProcessHelper
{
    public static async Task<int> RunProcessWithTimeoutAsync(string commandLine, TimeSpan timeout, CancellationToken token)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {commandLine}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (_, args) => Console.WriteLine(args.Data);
        process.ErrorDataReceived += (_, args) => Console.Error.WriteLine(args.Data);
        process.Start();

        var timeoutTokenSource = new CancellationTokenSource(timeout);
        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutTokenSource.Token);
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync(linkedTokenSource.Token);
        return process.ExitCode;
    }        
}
