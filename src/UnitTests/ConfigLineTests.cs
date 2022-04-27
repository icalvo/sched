using System;
using System.Collections.Generic;
using Cronos;
using FluentAssertions;
using Scheduler.ConfigParser;
using Scheduler.Intervals;
using Xunit;

namespace Scheduler.Tests;

public class ConfigLineTests
{
    public static readonly IEnumerable<object[]> ParseDateHeader =
        new[]
        {
            new object[] { "- 2022-01-02", Interval.Between(DateOnly.Parse("2022-01-02"), DateOnly.Parse("2022-01-03")) },
            new object[] { "- 2022-01-02 2022-01-05", Interval.Between(DateOnly.Parse("2022-01-02"), DateOnly.Parse("2022-01-06")) },
            new object[] { "- 2022-01-02 ...", Interval.From(DateOnly.Parse("2022-01-02")) },
            new object[] { "- ... 2022-01-02", Interval.To(DateOnly.Parse("2022-01-03")) },
            new object[] { "- ...", Interval.All<DateOnly>() },
        };

    [Theory]
    [MemberData(nameof(ParseDateHeader))]
    public void Parse_DateHeader(string line, Interval<DateOnly> expected)
    {
        ConfigLine result = ConfigLine.Parse(line);

        result.Should().BeOfType<IntervalHeader>().Subject
            .Interval.Start.Should().Be(expected.Start);
        result.Should().BeOfType<IntervalHeader>().Subject
            .Interval.End.Should().Be(expected.End);
    }
    public static readonly IEnumerable<object[]> ParsePeriodicTask =
        new[]
        {
            new object[] { "* * * 09 00 in", new TaskConfig("in", CronExpression.Parse("0 9 * * *")) },
            new object[] { "* * * 09 00 ls -l", new TaskConfig("ls -l", CronExpression.Parse("0 9 * * *")) },
        };

    [Theory]
    [MemberData(nameof(ParsePeriodicTask))]
    public void Parse_PeriodicTask(string line, TaskConfig expected)
    {
        ConfigLine result = ConfigLine.Parse(line);

        result.Should().BeOfType<TaskConfig>().Subject
            .Expression.Should().Be(expected.Expression);
        result.Should().BeOfType<TaskConfig>().Subject
            .CommandSpec.Should().Be(expected.CommandSpec);
    }
}