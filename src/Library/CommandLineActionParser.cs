namespace Scheduler;

public class CommandLineActionParser : IActionParser
{
    private readonly TimeSpan _processTimeout;

    public CommandLineActionParser(TimeSpan processTimeout)
    {
        _processTimeout = processTimeout;
    }

    public Func<CancellationToken, Task<int>> ParseAction(string commandLine)
        => token => ProcessHelper.RunProcessWithTimeoutAsync(commandLine, _processTimeout, token);
}
