namespace Scheduler;

public static class Notifications
{
    public static async Task NotifyAsync(
        string eventTitle,
        string[] commandFormats,
        CancellationToken token,
        params object[] commandArgs)
    {
        Console.WriteLine(eventTitle);

        var exitCodes = new List<int>();
        foreach (var commandFormat in commandFormats)
        {
            exitCodes.Add(await ProcessHelper.RunProcessWithTimeoutAsync(
                string.Format(commandFormat, commandArgs),
                TimeSpan.FromMinutes(1),
                token));
        }

        if (exitCodes.Any(exitCode => exitCode != 0))
        {
            Console.WriteLine($"There was an error when notifying {eventTitle}");
        }
    }
}
