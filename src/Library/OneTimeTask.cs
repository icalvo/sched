namespace Scheduler;

public record OneTimeTask(string ActionId, Func<CancellationToken, Task> Action, DateTimeOffset Time, TimeSpan RandomVariance)
{
    public DateTimeOffset RandomizedTime()
    {
        var ticks = RandomVariance.Ticks;
        return Time.AddTicks(Random.Shared.NextInt64(-ticks, ticks));
    }

    public async Task WaitAndRunAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.Now;
        if (now > Time) return;
        var randomizedTime = RandomizedTime();
        if (randomizedTime < now)
            randomizedTime = now;
        Console.WriteLine($"Waiting for {ActionId} {Time}, randomized at {randomizedTime}");
        TimeSpan waitingTime = randomizedTime - now;
        Console.WriteLine($"Waiting time: {waitingTime}");
        
        await Task.Delay(waitingTime, cancellationToken);
        Console.WriteLine($"Executing {ActionId}");
        await Action(cancellationToken);
    }
}
