using System.Collections.Concurrent;

namespace Scheduler;

public class NotificationsOptions
{
    public string NextEvent { get; set; }
    public string ConfigurationChanged { get; set; }
    public string ConfigurationChangeError { get; set; }
}

public class Application
{
    private readonly NotificationsOptions _options;

    public Application(NotificationsOptions options)
    {
        _options = options;
    }

    public Task RunAsync(string configPath)
    {
        return RunAsync(configPath, new CommandLineActionParser());
    }

    public Task MockAsync(string configPath)
    {
        Console.WriteLine("Mock run (commands will NOT be run!)");        
        return RunAsync(configPath, new MockActionParser());
    }

    private async Task RunAsync(string configPath, IActionParser actionParser)
    {
        var configChangeEvents = new BlockingCollection<int>();
        var directoryName = Path.GetDirectoryName(configPath) ?? throw new InvalidOperationException();
        using var watcher = new FileSystemWatcher(directoryName, Path.GetFileName(configPath));
        watcher.Created += (_, _) => configChangeEvents.Add(0);
        watcher.Changed += (_, _) => configChangeEvents.Add(0);

        var formerCts = new CancellationTokenSource();
        Task formerTask = await ReadConfigAndWaitEventsAsync(configPath, actionParser, formerCts.Token);            

        foreach (var _ in configChangeEvents.GetConsumingEnumerable())
        {
            var newCts = new CancellationTokenSource();
            formerCts.Cancel();
            await formerTask.WaitAsync(CancellationToken.None);
    
            Task newTask = await ReadConfigAndWaitEventsAsync(configPath, actionParser, newCts.Token);

            formerCts = newCts;
            formerTask = newTask;
        }
    }

    private async Task<Task> ReadConfigAndWaitEventsAsync(string configPath, IActionParser actionParser, CancellationToken token)
    {
        Task task;
        try
        {
            var (_, _, events) = EventBuilder.BuildEvents(actionParser, configPath);
            await NotifyConfigurationChangedAsync();
            task = Task.Run(async () =>
            {
                foreach (var ev in events)
                {
                    try
                    {
                        await NotifyNextEventAsync(ev);
                        await ev.WaitAndRunAsync(token);
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("Config changed, restarting...");
                        break;
                    }
                }
            }, token);
        }
        catch (Exception ex)
        {
            await NotifyConfigurationErrorAsync(ex);
            task = Task.CompletedTask;
        }

        return task;
    }

    private async Task NotifyNextEventAsync(OneTimeTask oneTimeTask)
    {
        var parser = new CommandLineActionParser();
        var action = parser.ParseAction(string.Format(_options.NextEvent, oneTimeTask.ActionId + " " + oneTimeTask.RandomizedTime));

        var exitCode = await action(CancellationToken.None);
        if (exitCode != 0)
        {
            Console.WriteLine("There was an error when notifying Next Event");
        }
    }

    private async Task NotifyConfigurationErrorAsync(Exception exception)
    {
        var parser = new CommandLineActionParser();
        var action = parser.ParseAction(string.Format(_options.ConfigurationChangeError, exception.Message));

        var exitCode = await action(CancellationToken.None);
        if (exitCode != 0)
        {
            Console.WriteLine("There was an error when notifying Next Event");
        }
    }

    private async Task NotifyConfigurationChangedAsync()
    {
        var parser = new CommandLineActionParser();
        var action = parser.ParseAction(string.Format(_options.ConfigurationChanged));
        var exitCode = await action(CancellationToken.None);
        if (exitCode != 0)
        {
            Console.WriteLine("There was an error when notifying Next Event");
        }
    }
}
