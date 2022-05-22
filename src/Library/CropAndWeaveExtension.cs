using Scheduler.Intervals;

namespace Scheduler;

public static class CropAndWeaveExtension
{
    public static IEnumerable<OneTimeTask> CropAndWeave(this IntervalElement<DateOnly, List<PeriodicTask>> interval, DateTimeOffset now, TimeSpan randomVariance)
    {
        DateTimeOffset ToDateTimeOffset(DateOnly dateOnly)
        {
            var midnight = TimeOnly.MinValue;
            return new DateTimeOffset(dateOnly.ToDateTime(midnight));
        }

        var start = interval
            .Start
            .MapReduce(x =>
                {
                    var dateTimeOffset = ToDateTimeOffset(x);
                    return dateTimeOffset > now ? dateTimeOffset : now;
                },
                now);
        var end = ToDateTimeOffset(interval.End.Reduce(DateOnly.MaxValue));

        if (start >= end)
        {
            return Enumerable.Empty<OneTimeTask>();
        }

        var periodicTasks = interval.Element;
        var randomVarianceTicks = randomVariance.Ticks;
        return 
            periodicTasks
                .Select(x => x
                    .Expression
                    .GetOccurrences(start, end, TimeZoneInfo.Local)
                    .Select(time =>
                    {
                        var randomizedTime = time.AddTicks(Random.Shared.NextInt64(-randomVarianceTicks, randomVarianceTicks));

                        return new OneTimeTask(x.Description, x.Action, time, randomizedTime);
                    }))
                .Weave(a => a.Time);
    }
}
