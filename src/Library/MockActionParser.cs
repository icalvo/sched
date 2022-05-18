namespace Scheduler;

public class MockActionParser : IActionParser
{
    public Func<CancellationToken, Task<int>> ParseAction(string commandLine) =>
        _ =>
        {
            Console.WriteLine($"Done action {commandLine}");
            return Task.FromResult(0);
        };
}
