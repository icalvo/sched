using Scheduler.Options;

namespace Scheduler.Intervals;

public class Endpoint<T> where T : IComparable<T>
{
    public Endpoint(ComparisonOperator comparison, Option<T> item)
    {
        Comparison = comparison;
        Item = item;
    }

    public ComparisonOperator Comparison { get; }
    public Option<T> Item { get; }

    public bool Contains(Option<T> item) =>
        item.MapReduce(it2 =>
            Item.MapReduce(it1 => 
                Comparison switch
                {
                    ComparisonOperator.LessThan => it2.CompareTo(it1) < 0,
                    ComparisonOperator.GreaterThan => it2.CompareTo(it1) > 0,
                    ComparisonOperator.LessOrEqualThan => it2.CompareTo(it1) <= 0,
                    ComparisonOperator.GreaterOrEqualThan => it2.CompareTo(it1) >= 0,
                    ComparisonOperator.None => throw new ArgumentOutOfRangeException(nameof(item)),
                    _ => throw new ArgumentOutOfRangeException(nameof(item))
                },
                true),
            true);
    
    public static implicit operator Option<T>(Endpoint<T> d) => d.Item;

    public override string ToString()
    {
        var itRep = Item.MapReduce(x => x.ToString(), "...");
        return Comparison switch
        {
            ComparisonOperator.LessThan => $"{itRep})",
            ComparisonOperator.GreaterThan => $"({itRep}",
            ComparisonOperator.LessOrEqualThan => $"{itRep}]",
            ComparisonOperator.GreaterOrEqualThan => $"[{itRep}",
            ComparisonOperator.None => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}