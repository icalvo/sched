namespace Scheduler;

public record OneTimeTask
{
    public OneTimeTask(string actionId, Func<CancellationToken, Task<int>> action, DateTimeOffset time, TimeSpan randomVariance)
    {
        ActionId = actionId;
        Action = action;
        Time = time;
        RandomVariance = randomVariance;
        var ticks = randomVariance.Ticks;
        RandomizedTime = time.AddTicks(Random.Shared.NextInt64(-ticks, ticks));
    }

    public DateTimeOffset RandomizedTime { get; }

    public async Task WaitAndRunAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.Now;
        if (now > Time) return;
        var randomizedTime = RandomizedTime;
        if (randomizedTime < now)
            randomizedTime = now;
        Console.WriteLine($"Waiting for {ActionId} {Time}, randomized at {randomizedTime}");
        TimeSpan waitingTime = randomizedTime - now;
        Console.WriteLine($"Waiting time: {waitingTime}");
        
        await TaskEx.RealTimeDelay(waitingTime, TimeSpan.FromSeconds(1), cancellationToken);
        Console.WriteLine($"Executing {ActionId}");
        var exitCode = await Action(cancellationToken);
        if (exitCode != 0)
        {
            Console.WriteLine($"The command failed with exit code {exitCode}");
        }
    }

    public string ActionId { get; }
    public Func<CancellationToken, Task<int>> Action { get; }
    public DateTimeOffset Time { get; }
    public TimeSpan RandomVariance { get; }

    public void Deconstruct(out string ActionId, out Func<CancellationToken, Task<int>> Action, out DateTimeOffset Time, out TimeSpan RandomVariance)
    {
        ActionId = this.ActionId;
        Action = this.Action;
        Time = this.Time;
        RandomVariance = this.RandomVariance;
    }
}
