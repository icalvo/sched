namespace Scheduler;

public record OneTimeTask(string ActionId, Func<CancellationToken, Task<int>> Action, DateTimeOffset Time, DateTimeOffset RandomizedTime)
{
    public async Task<int> WaitAndRunAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.Now;
        var randomizedTime = RandomizedTime;
        if (randomizedTime < now)
        {
            randomizedTime = now;
        }

        Console.WriteLine($"Waiting for {ActionId} {Time}, randomized at {randomizedTime}");
        TimeSpan waitingTime = randomizedTime - now;
        Console.WriteLine($"Waiting time: {waitingTime}");
        
        await TaskEx.RealTimeDelay(waitingTime, TimeSpan.FromSeconds(1), cancellationToken);
        Console.WriteLine($"Executing {ActionId}");
        
        var exitCode = await Action(cancellationToken);
        return exitCode;
    }
}
