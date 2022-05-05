namespace Scheduler;

public static class Application
{
    public static Task RunAsync(string configPath)
    {
        return RunAsync(configPath, new CommandLineActionParser());
    }

    public static Task MockAsync(string configPath)
    {
        Console.WriteLine("Mock run (commands will NOT be run!)");        
        return RunAsync(configPath, new MockActionParser());
    }

    private static async Task RunAsync(string configPath, IActionParser actionParser)
    {
        {
            while (true)
            {
                var directoryName = Path.GetDirectoryName(configPath) ?? throw new InvalidOperationException();
                using var watcher = new FileSystemWatcher(directoryName, Path.GetFileName(configPath));
                using var cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;
                watcher.Changed += (_, _) => cts.Cancel();
                watcher.EnableRaisingEvents = true;
                IEnumerable<OneTimeTask> events2;
                while (true)
                {
                    try
                    {
                        (_, _, events2) = EventBuilder.BuildEvents(actionParser, configPath);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Thread.Sleep(1000);
                    }
                }

                foreach (var ev in events2)
                {
                    try
                    {
                        await ev.WaitAndRunAsync(token);
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("Config changed, restarting...");
                        break;
                    }
                }
            }
        }
    }    
}
