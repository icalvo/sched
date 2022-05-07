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

    protected bool Equals(PeriodicTask other)
    {
        return Description == other.Description && Expression.Equals(other.Expression);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((PeriodicTask)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Description, Expression);
    }

    public override string ToString()
    {
        return Expression + " " + Description;
    }
}
