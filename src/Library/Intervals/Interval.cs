using Scheduler.Options;

namespace Scheduler.Intervals;
using static Option;

public static class Interval
{
    public static Interval<T> Create<T>(Option<T> from, Option<T> to)
        where T : IComparable<T>
    {
        return new Interval<T>(from, to);
    }

    public static Interval<T> Between<T>(T from, T to)
        where T : IComparable<T>
    {
        return Interval<T>.Between(from, to);
    }

    public static Interval<T> From<T>(T from)
        where T : IComparable<T>
    {
        return Interval<T>.From(from);
    }

    public static Interval<T> To<T>(T to)
        where T : IComparable<T>
    {
        return Interval<T>.To(to);
    }
    
    public static Interval<T> All<T>()
        where T : IComparable<T>
    {
        return Interval<T>.All();
    }
}

public class Interval<T> where T: IComparable<T>
{
    private Endpoint<T> StartEndpoint { get; }
    private Endpoint<T> EndEndpoint { get; }
    public Option<T> Start => StartEndpoint;
    public Option<T> End => EndEndpoint;

    public Interval(Option<T> from, Option<T> to)
    {
        StartEndpoint = from
            .MapReduce(
                f => new Endpoint<T>(ComparisonOperator.GreaterOrEqualThan, f),
                new Endpoint<T>(ComparisonOperator.GreaterThan, None));
        EndEndpoint = to
            .MapReduce(
                t => new Endpoint<T>(ComparisonOperator.LessThan, t),
                new Endpoint<T>(ComparisonOperator.LessThan, None));
    }
    public static Interval<T> Between(T from, T to)
    {
        return new Interval<T>(from, to);
    }

    public static Interval<T> From(T from)
    {
        return new Interval<T>(from, None);
    }

    public static Interval<T> To(T to)
    {
        return new Interval<T>(None, to);
    }


    public static Interval<T> All()
    {
        return new Interval<T>(None, None);
    }

    protected Interval(Interval<T> interval)
    {
        StartEndpoint = interval.StartEndpoint;
        EndEndpoint = interval.EndEndpoint;
    }

    public bool Contains(T date)
    {
        return
            StartEndpoint.Contains(date) &&
            EndEndpoint.Contains(date);
    }

    private bool ContainsEndpoint(Endpoint<T> t, Endpoint<T> endpoint)
    {
        return endpoint.Item.MapReduce(Contains, t.Item.IsNone);
    }

    public bool Contains(Interval<T> other)
    {
        return ContainsEndpoint(StartEndpoint, other.StartEndpoint) &&
               ContainsEndpoint(EndEndpoint, other.EndEndpoint);
    }

    public bool IsDisjointWith(Interval<T> other)
    {
        return !EndEndpoint.Contains(other.StartEndpoint) ||
               !StartEndpoint.Contains(other.EndEndpoint);
    }
    
    public bool Overlaps(Interval<T> other)
    {
        return EndEndpoint.Contains(other.StartEndpoint) &&
               StartEndpoint.Contains(other.EndEndpoint);
    }

    public override string ToString()
    {
        return $"{StartEndpoint}, {EndEndpoint}";
    }
}