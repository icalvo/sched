namespace Scheduler;

public class MockActionParser : IActionParser
{
    public Func<CancellationToken, Task> ParseAction(string commandLine) =>
        _ =>
        {
            Console.WriteLine($"Done action {commandLine}");
            return Task.CompletedTask;
        };
}
