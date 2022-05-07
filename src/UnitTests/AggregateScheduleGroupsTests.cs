using System;
using System.Linq;
using System.Threading.Tasks;
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

        result.Should().BeEquivalentTo(new[]{
            new DateIntervalList<PeriodicTask>(
                Interval<DateOnly>.All(),
                new []
                {
                    new PeriodicTask("in (triki in)", _ => Task.CompletedTask, CronExpression.Parse("0 9 * * *")),
                    new PeriodicTask("out (triki out)", _ => Task.CompletedTask, CronExpression.Parse("0 18 * * *"))
                }),
            new DateIntervalList<PeriodicTask>(
                Interval<DateOnly>.From(DateOnly.Parse("2022-02-03")),
                new []
                {
                    new PeriodicTask("in (triki in)", _ => Task.CompletedTask, CronExpression.Parse("0 10 * * *")),
                    new PeriodicTask("out (triki out)", _ => Task.CompletedTask, CronExpression.Parse("0 16 * * *"))
                })
            });
    }
}
