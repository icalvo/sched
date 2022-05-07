using Scheduler.Options;

namespace Scheduler.Intervals;
using static Option;

public static class IntervalElement {
    public static IntervalElement<T, TElement> Create<T, TElement>(Option<T> from, Option<T> to, TElement element)
        where T : IComparable<T>
    {
        return new IntervalElement<T, TElement>(Interval.Create(from, to), element);
    }
    
    public static IntervalElement<T, TElement> Between<T, TElement>(T from, T to, TElement element)
        where T : IComparable<T>
    {
        return new IntervalElement<T, TElement>(Interval.Between(from, to), element);
    }
    
    public static IntervalElement<T, TElement> From<T, TElement>(T from, TElement element)
        where T : IComparable<T>
    {
        return new IntervalElement<T, TElement>(Interval.From(from), element);
    }    
    
    public static IntervalElement<T, TElement> To<T, TElement>(T to, TElement element)
        where T : IComparable<T>
    {
        return new IntervalElement<T, TElement>(Interval.To(to), element);
    }

    public static IEnumerable<IntervalElement<T, TElement>> Flatten<T, TElement>(
        this IEnumerable<IntervalElement<T, TElement>> ranges,
        TElement defaultElement) where T : IComparable<T>
    {
        return ranges
            .Aggregate(
                EnumerableExtensions.From(new IntervalElement<T, TElement>(Interval.All<T>(), defaultElement)),
                (existing, newInterval) => OverlapRange(existing, newInterval).ToList())
            .ToList();
    }

    public static IEnumerable<IntervalElement<T, TElement>> Flatten<T, TElement>(
        this IEnumerable<IntervalElement<T, TElement>> ranges)
        where T : IComparable<T> where TElement : new()
    {
        return ranges.Flatten(new TElement());
    }
    private static IEnumerable<IntervalElement<T, TElement>> OverlapRange<T, TElement>(
        IEnumerable<IntervalElement<T, TElement>> flattened,
        IntervalElement<T, TElement> newInterval) where T : IComparable<T>
    {
        bool IsCorrectInterval(Option e1, Option e2)
        {
            return (e1.IsSome || e2.IsSome) && !e1.Equals(e2);
        }

        const int before = 1;
        const int during = 2;
        const int after = 3;

        var status = before;
        
        foreach (var existingInterval in flattened)
        {
            switch (status)
            {
                case before:
                    if (!existingInterval.Overlaps(newInterval))
                    {
                        yield return existingInterval;
                    }
                    else if (existingInterval.Contains(newInterval))
                    {
                        if (IsCorrectInterval(existingInterval.Start, newInterval.Start))
                        {
                            yield return Create(existingInterval.Start, newInterval.Start, existingInterval.Element);
                        }

                        yield return newInterval;

                        if (IsCorrectInterval(newInterval.End, existingInterval.End))
                        {
                            yield return Create(newInterval.End, existingInterval.End, existingInterval.Element);
                        }

                        status = after;
                    }
                    else
                    {
                        if (IsCorrectInterval(existingInterval.Start, newInterval.Start))
                        {
                            yield return Create(existingInterval.Start, newInterval.Start, existingInterval.Element);
                        }

                        yield return newInterval;
                        status = during;
                    }
                    break;
                case during:
                    if (!newInterval.Contains(existingInterval))
                    {
                        if (IsCorrectInterval(newInterval.End, existingInterval.End))
                        {
                            yield return Create(newInterval.End, existingInterval.End, existingInterval.Element);
                        }

                        status = after;
                    }
                    break;
                case after:
                    yield return existingInterval;
                    break;
            }
        }
    }    
}

public static class IntervalElement<T> where T : IComparable<T>
{
    
    public static IntervalElement<T, TElement> All<TElement>(TElement element)
    {
        return IntervalElement.Create<T, TElement>(None, None, element);
    }

}
public class IntervalElement<T, TElement> : Interval<T> where T : IComparable<T>
{
    public IntervalElement(Interval<T> interval, TElement element) : base(interval)
    {
        Element = element;
    }

    public TElement Element { get; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((IntervalElement<T, TElement>)obj);
    }

    private bool Equals(IntervalElement<T, TElement> other)
    {
        return EqualityComparer<TElement>.Default.Equals(Element, other.Element) &&
               Start.Equals(other.Start) &&
               End.Equals(other.End);
    }

    public override int GetHashCode()
    {
        return (Element == null ? 0 : EqualityComparer<TElement>.Default.GetHashCode(Element)) ^
               Start.GetHashCode() ^
               End.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString() + " " + ElementToString();
    }

    protected virtual string? ElementToString()
    {
        return Element?.ToString();
    }
}
