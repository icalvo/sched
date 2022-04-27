using Scheduler.Options;

namespace Scheduler.Intervals;

/// <summary>
/// Interval in which the element is a list.
/// </summary>
public class IntervalList<T, TElement> : IntervalElement<T, List<TElement>> where T : IComparable<T>
{
    public IntervalList(Option<T> from, Option<T> to, List<TElement> element)
        : base(Interval.Create(from, to), element)
    {
    }

    public IntervalList(Interval<T> interval)
        : base(interval, new List<TElement>())
    {
    }
}