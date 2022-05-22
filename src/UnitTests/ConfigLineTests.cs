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
    public static readonly IEnumerable<object[]> ParseSuccessData =
        new []
        {
            new object[] { "    2022-01-02", new IntervalHeader(Interval.Between(DateOnly.Parse("2022-01-02"), DateOnly.Parse("2022-01-03"))) },
            new object[] { "  2022-01-02   2022-01-05  ", new IntervalHeader(Interval.Between(DateOnly.Parse("2022-01-02"), DateOnly.Parse("2022-01-06"))) },
            new object[] { "2022-01-02 ... ", new IntervalHeader(Interval.From(DateOnly.Parse("2022-01-02"))) },
            new object[] { "  ... 2022-01-02", new IntervalHeader(Interval.To(DateOnly.Parse("2022-01-03"))) },
            new object[] { "    ...", new IntervalHeader(Interval.All<DateOnly>()) },
            new object[] { "* *   * 09 00      in", new TaskConfig("in", CronExpression.Parse("0 9 * * *")) },
            new object[] { "*   * * 09    00 ls -l", new TaskConfig("ls -l", CronExpression.Parse("0 9 * * *")) },
            new object[] { "R 00:01:30", new RandomizeConfig(new TimeSpan(0, 0, 1, 30)) },
            new object[] { "R  02:05:00   ", new RandomizeConfig(new TimeSpan(0, 2, 5, 0)) },
            new object[] { "- 2022-01-02", new IntervalHeader(Interval.Between(DateOnly.Parse("2022-01-02"), DateOnly.Parse("2022-01-03"))) },
            new object[] { "-   2022-01-02 2022-01-05", new IntervalHeader(Interval.Between(DateOnly.Parse("2022-01-02"), DateOnly.Parse("2022-01-06"))) },
            new object[] { "-  2022-01-02 ...", new IntervalHeader(Interval.From(DateOnly.Parse("2022-01-02"))) },
            new object[] { "- ... 2022-01-02", new IntervalHeader(Interval.To(DateOnly.Parse("2022-01-03"))) },
            new object[] { "- ...", new IntervalHeader(Interval.All<DateOnly>()) },
            new object[] { "-", new IntervalHeader(Interval.All<DateOnly>()) },
        };

    [Theory]
    [MemberData(nameof(ParseSuccessData))]
    public void Parse_Success(string line, ConfigLine expected)
    {
        ConfigLine result = ConfigLine.Parse(line, 0);

        result.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }

    [Theory]
    [InlineData("x")]
    [InlineData(" - 2022-02-02")]
    [InlineData(" - 2022-32-02")]
    [InlineData("- 2022-02-02 2022-02-07 2022-01-02")]
    [InlineData(" R 00:01:00")]
    [InlineData("* * * dir")]
    [InlineData("x * * * * dir")]
    public void Parse_Empty(string line)
    {
        var action = () => ConfigLine.Parse(line, 13);

        action.Should().Throw<Exception>().WithMessage($"Failed parsing line 14: {line}");
    }
}
