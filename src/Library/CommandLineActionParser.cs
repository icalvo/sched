using System.Diagnostics;

namespace Scheduler;

public class CommandLineActionParser : IActionParser
{
    public Func<CancellationToken, Task> ParseAction(string commandLine)
    {
        return _ =>
        {
            var process = new Process
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

            process.Start();
 
            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
            
            return Task.CompletedTask;
        };
    }
}
