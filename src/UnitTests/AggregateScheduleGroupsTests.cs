using System;
using System.Linq;
using Cronos;
using FluentAssertions;
using Scheduler.ConfigParser;
using Scheduler.Intervals;
using Xunit;

namespace Scheduler.Tests;

public class AggregateScheduleGroupsTests
{
    [Fact]
    public void Test()
    {
        var lines = new ConfigLine[]
        {
            new AliasConfig("in", "triki in"),
            new AliasConfig("out", "triki out"),
            new TaskConfig("in", CronExpression.Parse("0 9 * * *")),
            new TaskConfig("out", CronExpression.Parse("0 18 * * *")),
            new IntervalHeader(Interval.From(DateOnly.Parse("2022-02-03"))),
            new TaskConfig("in", CronExpression.Parse("0 10 * * *")),
            new TaskConfig("out", CronExpression.Parse("0 16 * * *")),
        };

        var result = lines.AggregateScheduleGroups(new MockActionParser()).ToList();

        result.Should().HaveCount(2);
        result[0].Start.IsSome.Should().BeFalse();
        result[0].End.IsSome.Should().BeFalse();
        result[0].Element.Should().HaveCount(2);
        result[1].Start.IsSome.Should().BeTrue();
        result[1].End.IsSome.Should().BeFalse();
        result[1].Element.Should().HaveCount(2);
    }
}
