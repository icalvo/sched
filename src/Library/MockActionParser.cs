namespace Scheduler;

public class MockActionParser : IActionParser
{
    public Func<CancellationToken, Task> ParseAction(string actionId) =>
        _ =>
        {
            Console.WriteLine($"Done action {actionId}");
            return Task.CompletedTask;
        };
}
