using System.Reflection;
using Microsoft.Extensions.Configuration;
using Scheduler;
using SimpleCommandLine;

const string countArgumentName = "count";
const string configFileArgumentName = "config_file";
const string mockOptionName = "--mock";

const string environmentVarPrefix = "Sched_";
const string configFolderEnvironment = "GlobalConfigurationFolder";
const string userProfileFolder = ".sched";

// The user's appsettings.json will be located at a folder defined in "Sched_GlobalConfigurationFolder" or,
// if absent, in ~/.sched/ (e.g. C:\Users\name\.sched\appsettings.json
var initConfig = new ConfigurationBuilder().AddEnvironmentVariables(prefix: environmentVarPrefix).Build();
var globalUserConfigFolder = 
    initConfig[configFolderEnvironment]
    ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), userProfileFolder);
var globalUserConfigPath = Path.Combine(globalUserConfigFolder, "appsettings.json");

// Configuration takes values from:
// - The appsettings.json in the installation path (must be present and it is NOT recommended to edit it.)
// - The user's appsettings.json (it is optional and defined in the former lines.)
// - The environment variables (with the prefix Sched_).
IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile(globalUserConfigPath, optional: true, reloadOnChange: false)
    .AddEnvironmentVariables(prefix: environmentVarPrefix)
    .Build();

var notificationOptions =
    config.GetSection(nameof(NotificationsOptions))
    .Get<NotificationsOptions>()
    ?? throw new InvalidOperationException($"Could not find a {nameof(NotificationsOptions)} section in your configuration.");

var application = new Application(
    notificationOptions,
    new CommandLineActionParser(config.GetValue<TimeSpan>("ProcessTimeout")),
    new MockActionParser());

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
            await application.MockAsync(configPath);
        }
        else
        {
            await application.RunAsync(configPath);
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
    new Option[]
        {
            new("--random", "Apply randomization config and show randomized times", "-r", "--rnd")
        },
    (command, options, arguments) =>
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

        Show(count, configPath, options.ContainsKey("--random"));

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

static void Show(int count, string configPath, bool randomize)
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

    if (randomize)
    {
        Console.WriteLine($"Next {count} events (with randomization):");
        foreach (var ev in events.Take(count))
        {
            Console.WriteLine($"{ev.RandomizedTime} -> {ev.ActionId}");
        }
    }
    else
    {
        Console.WriteLine($"Next {count} events:");
        foreach (var ev in events.Take(count))
        {
            Console.WriteLine($"{ev.Time} -> {ev.ActionId}");
        }
    }
}

