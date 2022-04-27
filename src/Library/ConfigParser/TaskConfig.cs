using Cronos;

namespace Scheduler.ConfigParser;

/// <summary>
/// Scheduled action. The schedule is provided by a Cron expression.
/// </summary>
public class TaskConfig : ConfigLine
{
    public TaskConfig(string actionId, CronExpression expression)
    {
        CommandSpec = actionId;
        Expression = expression;
    }

    public string CommandSpec { get; }
    public CronExpression Expression { get; }    
}