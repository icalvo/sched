using System.Diagnostics;

namespace Scheduler;

public class CommandLineActionParser : IActionParser
{
    public Func<CancellationToken, Task<int>> ParseAction(string commandLine)
    {
        return async token =>
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {commandLine}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += (_, args) => Console.WriteLine(args.Data);
            process.ErrorDataReceived += (_, args) => Console.Error.WriteLine(args.Data);
            process.Start();
            await process.WaitForExitAsync(token);
            return process.ExitCode;
        };
    }
}
