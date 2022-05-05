using Scheduler.ConfigParser;

namespace Scheduler;

public interface IActionParser
{
    public Func<CancellationToken, Task> ParseAction(string commandLine);
    

    public PeriodicTask Convert(TaskConfig taskConfig, IReadOnlyDictionary<string, string> aliases)
    {
        _ = aliases.TryGetValue(taskConfig.CommandSpec, out var commandLine);

        string description;
        if (commandLine == null)
        {
            commandLine = taskConfig.CommandSpec;
            description = taskConfig.CommandSpec;
        }
        else
        {
            description = $"{taskConfig.CommandSpec} ({commandLine})";
        }

        return new PeriodicTask(description, ParseAction(commandLine), taskConfig.Expression);
    }
}
