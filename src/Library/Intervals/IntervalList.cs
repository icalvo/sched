using Scheduler.Options;

namespace Scheduler.Intervals;

/// <summary>
/// Interval in which the element is a list.
/// </summary>
public class IntervalList<T, TElement> : IntervalElement<T, List<TElement>> where T : IComparable<T>
{
    public IntervalList(Option<T> from, Option<T> to, IEnumerable<TElement> elements)
        : base(Interval.Create(from, to), elements.ToList())
    {
    }

    public IntervalList(Interval<T> interval)
        : base(interval, new List<TElement>())
    {
    }

    protected IntervalList(Interval<T> interval, IEnumerable<TElement> elements)
        : base(interval, elements.ToList())
    {
    }

    protected override string ElementToString()
    {
        return Element.Select(x => x?.ToString()).StringJoin(", ");
    }


    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((IntervalList<T, TElement>)obj);
    }

    private bool Equals(IntervalList<T, TElement> other)
    {
        return Element.SequenceEqual(other.Element) &&
               Start.Equals(other.Start) &&
               End.Equals(other.End);
    }

    public override int GetHashCode()
    {
        return (EqualityComparer<List<TElement>>.Default.GetHashCode(Element)) ^
               Start.GetHashCode() ^
               End.GetHashCode();
    }
}
