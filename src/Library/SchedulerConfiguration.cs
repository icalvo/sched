using System.Collections.Immutable;
using Scheduler.ConfigParser;
using Scheduler.Intervals;

namespace Scheduler;

public record SchedulerConfiguration(ImmutableList<DateIntervalList<TaskConfig>> Groups, IImmutableDictionary<string, string> Aliases)
{
    public SchedulerConfiguration() : this(ImmutableList<DateIntervalList<TaskConfig>>.Empty, ImmutableDictionary<string, string>.Empty)
    {
    }
}