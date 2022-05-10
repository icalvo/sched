namespace Scheduler;

public static class TaskEx
{
    public static Task RealTimeDelay(TimeSpan delay) =>
        RealTimeDelay(delay, CancellationToken.None);


    public static Task RealTimeDelay(TimeSpan delay, TimeSpan precision) =>
        RealTimeDelay(delay, precision, CancellationToken.None);

    public static Task RealTimeDelay(TimeSpan delay, CancellationToken token) =>
        RealTimeDelay(delay, TimeSpan.FromMilliseconds(500), token);

    public static async Task RealTimeDelay(TimeSpan delay, TimeSpan precision, CancellationToken token)
    {
        DateTime start = DateTime.UtcNow;
        DateTime end = start + delay;

        while (DateTime.UtcNow < end)
        {
            await Task.Delay(precision, token);
        }
    }
}
