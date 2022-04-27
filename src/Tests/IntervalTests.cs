using FluentAssertions;
using Scheduler.Intervals;
using Scheduler.Options;
using Xunit;
using static Scheduler.Options.Option;

namespace Scheduler.Tests;

public class IntervalTests
{
    [Fact]
    public void Between()
    {
        var subject = Interval.Between(2, 5);
        subject.Start.Should().Be(Some(2));
        subject.End.Should().Be(Some(5));
    }

    [Fact]
    public void From()
    {
        var subject = Interval.From(2);
        subject.Start.Should().Be(Some(2));
        subject.End.Should().Be(None);
    }

    [Fact]
    public void To()
    {
        var subject = Interval.To(2);
        subject.Start.Should().Be(None);
        subject.End.Should().Be(Some(2));
    }

    [Fact]
    public void NoneString()
    {
        None.ToString().Should().Be("None");
    }

    [Fact]
    public void Create()
    {
        var subject = Interval.Create(Some(2), None);
        subject.Start.Should().Be(Some(2));
        subject.End.Should().Be(None);
    }

    [Theory]
    [InlineData(8, false)]
    [InlineData(10, true)]
    [InlineData(12, true)]
    [InlineData(28, false)]
    [InlineData(40, false)]
    public void ContainsItem(int value, bool expected)
    {
        var interval1 = new Interval<int>(10, 28);

        interval1.Contains(value).Should().Be(expected);
        interval1.Contains(10).Should().BeTrue();
        interval1.Contains(12).Should().BeTrue();
        interval1.Contains(28).Should().BeFalse();
        interval1.Contains(40).Should().BeFalse();
    }

    [Theory]
    [InlineData(10, 28, 12, 30, false, false, true, false)]
    [InlineData(10, 28, 8, 15, false, false, true, false)]
    [InlineData(10, 28, 12, 15, true, false, true, false)]
    [InlineData(10, 28, 5, 30, false, true, true, false)]
    [InlineData(10, 28, 40, 50, false, false, false, true)]
    [InlineData(10, 28, 10, 28, false, false, true, false)]
    [InlineData(null, 28, 12, 30, false, false, true, false)]
    [InlineData(null, null, null, null, true, true, true, false)]
    public void ContainsInterval(int? from1, int? to1, int? from2, int? to2, bool contains12, bool contains21,
        bool overlaps, bool disjoint)
    {
        var interval1 = new Interval<int>(from1.StructToOption(), to1.StructToOption());
        var interval2 = new Interval<int>(from2.StructToOption(), to2.StructToOption());

        interval1.Contains(interval2).Should().Be(contains12);
        interval2.Contains(interval1).Should().Be(contains21);
        interval1.Overlaps(interval2).Should().Be(overlaps);
        interval2.Overlaps(interval1).Should().Be(overlaps);
        interval1.IsDisjointWith(interval2).Should().Be(disjoint);
        interval2.IsDisjointWith(interval1).Should().Be(disjoint);
    }
}