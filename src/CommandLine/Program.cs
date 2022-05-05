using System.Reflection;
using Scheduler;
using SimpleCommandLine;

const string countArgumentName = "count";
const string configFileArgumentName = "config_file";
const string mockOptionName = "--mock";

var runCommand = new SubCommand(
    "run",
    "Runs or mock-runs a configuration file.",
    new[] { new Argument(configFileArgumentName, "Sched configuration file") },
    new[]
    {
        new Option(
            mockOptionName,
            "Instead of running the configured commands, it just outputs the command that would be executed.",
            "-m")
    }, 
    async (command, options, arguments) => {
        string configPath = arguments[configFileArgumentName];
        
        if (!File.Exists(configPath))
        {
            command.PrintErrorAndHelp("Config file must exist.");
            return 4;
        }

        if (options.ContainsKey(mockOptionName))
        {
            await Application.MockAsync(configPath);
        }
        else
        {
            await Application.RunAsync(configPath);
        }

        return 0;
    }
);

var showCommand = new SubCommand(
    "show",
    "Shows the next N events",
    new Argument[]
    {
        new(countArgumentName, "Number of events to show"),
        new(configFileArgumentName, "sched configuration file")
    },
    (command, _, arguments) =>
    {
        if (!int.TryParse(arguments[countArgumentName], out var count))
        {
            command.PrintErrorAndHelp("Count must be a number.");
            return 5;
        }

        if (count <= 0)
        {
            command.PrintErrorAndHelp("Count must be a greater than 0.");
            return 6;
        }

        var configPath = arguments[configFileArgumentName];
        if (!File.Exists(configPath))
        {
            command.PrintErrorAndHelp("Config file must exist.");
            return 4;
        }

        Show(count, configPath);

        return 0;
    });

var versionCommand = new SubCommand(
    "version",
    "Shows the program version",
    (_, _, _) =>
    {
        Console.WriteLine(Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion);
        return 0;
    });

var rootCommand = new RootCommand(
    "Advanced scheduling tools",
    runCommand,
    showCommand,
    versionCommand);

await rootCommand.RunAsync(args);

static void Show(int count, string configPath)
{
    IActionParser actionParser = new MockActionParser();

    Console.WriteLine("Raw groups:");
    var (scheduleGroups, flattenedGroups, events) = EventBuilder.BuildEvents(actionParser, configPath);
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
