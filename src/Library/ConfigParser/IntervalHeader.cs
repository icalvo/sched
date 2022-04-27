using Scheduler.Intervals;

namespace Scheduler.ConfigParser;

public class IntervalHeader : ConfigLine
{
    public IntervalHeader(Interval<DateOnly> interval)
    {
        Interval = interval;
    }

    public Interval<DateOnly> Interval { get; }
}