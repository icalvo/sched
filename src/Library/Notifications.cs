namespace Scheduler;

public static class Notifications
{
    public static async Task NotifyAsync(
        string eventTitle,
        string? commandFormat,
        CancellationToken token,
        params object[] commandArgs)
    {
        Console.WriteLine(eventTitle);
        if (commandFormat == null)
        {
            return;
        }

        var exitCode = await ProcessHelper.RunProcessWithTimeoutAsync(
            string.Format(commandFormat, commandArgs),
            TimeSpan.FromMinutes(1),
            token);

        if (exitCode != 0)
        {
            Console.WriteLine($"There was an error when notifying {eventTitle}");
        }
    }
}
