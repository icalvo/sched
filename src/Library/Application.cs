using System.Collections.Concurrent;

namespace Scheduler;

public class Application
{
    private readonly INotificationsOptions _options;
    private readonly IActionParser _runActionParser;
    private readonly IActionParser _mockActionParser;

    public Application(
        INotificationsOptions options,
        IActionParser runActionParser,
        IActionParser mockActionParser)
    {
        _options = options;
        _runActionParser = runActionParser;
        _mockActionParser = mockActionParser;
    }

    public Task RunAsync(string configPath)
    {
        return RunAsync(configPath, _runActionParser);
    }

    public Task MockAsync(string configPath)
    {
        Console.WriteLine("Mock run (commands will NOT be run!)");        
        return RunAsync(configPath, _mockActionParser);
    }

    private async Task RunAsync(string configPath, IActionParser actionParser)
    {
        var configChangeEvents = new BlockingCollection<int>();
        var directoryName = Path.GetDirectoryName(configPath) ?? throw new InvalidOperationException();
        using var watcher = new FileSystemWatcher(directoryName, Path.GetFileName(configPath));
        watcher.Created += (_, _) => configChangeEvents.Add(0);
        watcher.Changed += (_, _) => configChangeEvents.Add(0);
        watcher.EnableRaisingEvents = true;
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
            await NotifyConfigurationChangedAsync(token);
            task = Task.Run(async () =>
            {
                foreach (var ev in events)
                {
                    try
                    {
                        await NotifyNextEventAsync(ev, token);
                        var exitCode = await ev.WaitAndRunAsync(token);
                        if (exitCode == 0)
                        {
                            await Notifications.NotifyAsync($"Command {ev.ActionId} succeeded", _options.CommandSucceeded, token);
                        }
                        else
                        {
                            await Notifications.NotifyAsync($"Command {ev.ActionId} failed", _options.CommandFailed, token, exitCode);
                        }
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
            await NotifyConfigurationErrorAsync(ex, token);
            task = Task.CompletedTask;
        }

        return task;
    }

    private Task NotifyNextEventAsync(OneTimeTask oneTimeTask, CancellationToken token)
    {
        string eventInfo = $"{oneTimeTask.ActionId} {oneTimeTask.RandomizedTime}";
        return Notifications.NotifyAsync(
            $"Next event: {eventInfo}",
            _options.NextEvent,
            token,
            oneTimeTask.ActionId,
            oneTimeTask.RandomizedTime);
    }

    private Task NotifyConfigurationErrorAsync(Exception exception, CancellationToken token)
    {
        return Notifications.NotifyAsync(
            "Error parsing configuration",
            _options.ConfigurationChangeError,
            token,
            exception.Message);
    }

    private Task NotifyConfigurationChangedAsync(CancellationToken token)
    {
        return Notifications.NotifyAsync("Configuration loaded", _options.ConfigurationChanged, token);
    }
}
