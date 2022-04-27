using Scheduler;
using Scheduler.CommandLine;
using Scheduler.ConfigParser;
using Scheduler.Intervals;

switch (args[0])
{
    case "show":
    {
        Show(int.Parse(args[1]), args[2]);
    }

        break;
    case "run":
    case "mock":
    {
        IActionParser actionParser =
                args[0] == "run"
                    ? new CommandLineActionParser()
                    : new MockActionParser();

        await RunAsync(args[1], actionParser);
        break;
    }
        
    default:
        Console.WriteLine("Incorrect argument");
        break;
}

static void Show(int count, string configPath)
{
    IActionParser actionParser = new MockActionParser();

    Console.WriteLine("Raw groups:");
    var (scheduleGroups, flattenedGroups, events) = BuildEvents(actionParser, configPath);
    foreach (var scheduleGroup in scheduleGroups)
    {
        Console.WriteLine($"{scheduleGroup} ({scheduleGroup.Element.Count} cron lines)");
    }

    Console.WriteLine();

    Console.WriteLine("Flattened groups:");
    foreach (var group in flattenedGroups)
    {
        Console.WriteLine($"{group} ({group.Element.Count} cron lines)");
    }

    Console.WriteLine();

    Console.WriteLine($"Next {count} events:");
    foreach (var ev in events.Take(count))
    {
        Console.WriteLine($"{ev.Time} -> {ev.ActionId} (randomization example: {ev.RandomizedTime()})");
    }
}

static async Task RunAsync(string configPath, IActionParser actionParser)
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
                    (_, _, events2) = BuildEvents(actionParser, configPath);
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

static (
    List<DateIntervalList<PeriodicTask>> scheduleGroups,
    List<IntervalElement<DateOnly, List<PeriodicTask>>> flattenedGroups,
    IEnumerable<OneTimeTask> events) BuildEvents(IActionParser actionParser, string configPath)
{
    var now = DateTimeOffset.Now;

    string[]? lines = null;
    for (int i = 0; i < 5; i++)
    {
        try
        {
            lines = File.ReadAllLines(configPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Thread.Sleep(1000);
        }
    }

    if (lines == null) throw new Exception("Cannot read the file");

    var parsedLines = lines
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Where(line => !line.StartsWith("#"))
        .Select(ConfigLine.Parse);

    var config = parsedLines.AggregateScheduleGroups(actionParser);
    var scheduleGroups = config.ToList();
    var flattenedGroups = scheduleGroups.Flatten().ToList();
    var events = flattenedGroups.SelectMany(x => x.CropAndWeave(now));

    return (scheduleGroups, flattenedGroups, events);
}



