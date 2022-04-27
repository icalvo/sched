using Cronos;
using Scheduler.ConfigParser;

namespace Scheduler;

/// <summary>
/// Scheduled action. The schedule is provided by a Cron expression.
/// </summary>
public class PeriodicTask
{
    public PeriodicTask(string description, Func<CancellationToken, Task> action, CronExpression expression)
    {
        Description = description;
        Action = action;
        Expression = expression;
    }

    public string Description { get; }
    public Func<CancellationToken, Task> Action { get; }
    public CronExpression Expression { get; }    
}