namespace Scheduler;

public class CommandLineActionParser : IActionParser
{
    public Func<CancellationToken, Task> ParseAction(string actionId)
    {
        return _ => Task.CompletedTask;
    }
}