using FluentAssertions;
using Scheduler.Intervals;
using Xunit;

namespace Scheduler.Tests;

public class FlattenTests
{
    [Fact]
    public void Overlap()
    {
        var mix = new[]
        {
            IntervalElement<int>.All('Y'),
            IntervalElement.Between(10, 28, 'A'),
            IntervalElement.Between(10, 28, 'A'),
            IntervalElement.Between(12, 30, 'B'),
        }.Flatten('X');

        mix.Should().BeEquivalentTo(new[]
        {
            IntervalElement.To(10, 'Y'),
            IntervalElement.Between(10, 12, 'A'),
            IntervalElement.Between(12, 30, 'B'),
            IntervalElement.From(30, 'Y'),
        });
    }
    
    [Fact]
    public void Overlap2()
    {
        var mix = new[]
        {
            IntervalElement<int>.All('Y'),
            IntervalElement.Between(2, 3, 'A')
        }.Flatten('X');

        mix.Should().BeEquivalentTo(new[]
        {
            IntervalElement.To(2, 'Y'),
            IntervalElement.Between(2, 3, 'A'),
            IntervalElement.From(3, 'Y'),
        });
    }    
}