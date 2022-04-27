using Scheduler.Options;

namespace Scheduler.Intervals;

/// <summary>
/// Interval in which the element is a list.
/// </summary>
public class DateIntervalList<TElement> : IntervalList<DateOnly, TElement>
{
    public DateIntervalList(Option<DateOnly> from, Option<DateOnly> to, List<TElement> element)
        : base(from, to, element)
    {
    }

    public DateIntervalList(Interval<DateOnly> interval)
        : base(interval)
    {
    }
}