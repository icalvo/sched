using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Scheduler.Tests;

public class WeaveTests
{
    [Fact]
    public void Weave_EmptyCollectionsReturnEmptyCollection()
    {
        var a = Array.Empty<int>();
        var b = Array.Empty<int>();
        var actual = EnumerableExtensions.Weave(a, b);

        actual.Should().BeEmpty();
    }

    [Fact]
    public void Weave_Test1()
    {
        var a = new[] { ('A', 1), ('B', 3), ('C', 5) };
        var b = new[] { ('D', 4), ('E', 7) };
        var actual = EnumerableExtensions.Weave(x => x.Item2, a, b);

        actual.Should().Equal(('A', 1), ('B', 3), ('D', 4), ('C', 5), ('E', 7));
    }

    [Fact]
    public void Weave_Test2()
    {
        var a = new[] { ('A', 1), ('B', 3), ('C', 5) };
        var b = new[] { ('D', 4), ('E', 7) };
        var c = new[] { ('F', 3), ('G', 6) };
        var actual = EnumerableExtensions.Weave(x => x.Item2, a, b, c);

        actual.Should().Equal(('A', 1), ('B', 3), ('F', 3), ('D', 4), ('C', 5), ('G', 6), ('E', 7));
    }
    
    
    [Fact]
    public void Weave_Test3()
    {
        var a = new[] { ('A', 1), ('B', 3), ('C', 5) };
        var b = new[] { ('D', 4), ('E', 7) };
        var c = new[] { ('F', 3), ('G', 6) };
        var actual = EnumerableExtensions.Weave(x => x.Item2, a, b, c);

        actual.Should().Equal(('A', 1), ('B', 3), ('F', 3), ('D', 4), ('C', 5), ('G', 6), ('E', 7));
    }

    [Fact]
    public void Weave_SupportsInfiniteSequences()
    {
        var a = InfiniteSequenceFrom(0).Select(x => x * 3 + 1);
        var b = InfiniteSequenceFrom(0).Select(x => x * 3 + 2);
        var c = InfiniteSequenceFrom(0).Select(x => x * 3 + 3);
        
        var actual = EnumerableExtensions.Weave(a, b, c);

        actual.Take(10).Should().Equal(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
    }

    [SuppressMessage("ReSharper", "IteratorNeverReturns", Justification = "The point")]
    private static IEnumerable<int> InfiniteSequenceFrom(int start)
    {
        var accum = start;
        while (true)
        {
            yield return accum;
            accum++;
        }
    }
}

