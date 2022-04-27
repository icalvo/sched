namespace Scheduler;

public record OneTimeTask(string ActionId, Func<CancellationToken, Task> Action, DateTimeOffset Time)
{
    public DateTimeOffset RandomizedTime()
    {
        return Time.AddMinutes(Random.Shared.Next(-5, 6));
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