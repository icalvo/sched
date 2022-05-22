using System;
using System.Linq;
using System.Threading.Tasks;
using Cronos;
using FluentAssertions;
using Scheduler.Intervals;
using Scheduler.Options;
using Xunit;

namespace Scheduler.Tests;

public class CropAndWeaveTests
{
    [Fact]
    public void CropAndWeave_NowInsideInterval()
    {
        var now = DateTimeOffset.Parse("2022-01-03");
        var subject = new DateIntervalList<PeriodicTask>(
            DateOnly.Parse("2022-01-01"),
            DateOnly.Parse("2022-01-04").AddDays(1),
            new[]
            {
                new PeriodicTask("in", _ => Task.FromResult(0), CronExpression.Parse("0 9 * * *")),
                new PeriodicTask("out", _ => Task.FromResult(0), CronExpression.Parse("0 18 * * *"))
            }.ToList());
        var result = subject.CropAndWeave(now, TimeSpan.Zero);

        result.Select(x => (x.ActionId, x.Time)).Should().Equal(
            ("in", DateTimeOffset.Parse("2022-01-03 09:00")),
            ("out", DateTimeOffset.Parse("2022-01-03 18:00")),
            ("in", DateTimeOffset.Parse("2022-01-04 09:00")),
            ("out", DateTimeOffset.Parse("2022-01-04 18:00")));
    }

    [Fact]
    public void CropAndWeave_NowInsideIntervalJustAfter()
    {
        var now = DateTimeOffset.Parse("2022-01-03 09:01");
        var subject = new DateIntervalList<PeriodicTask>(
            DateOnly.Parse("2022-01-01"),
            DateOnly.Parse("2022-01-04").AddDays(1),
            new[]
            {
                new PeriodicTask("in", _ => Task.FromResult(0), CronExpression.Parse("0 9 * * *")),
                new PeriodicTask("out", _ => Task.FromResult(0), CronExpression.Parse("0 18 * * *"))
            }.ToList());
        var result = subject.CropAndWeave(now, TimeSpan.Zero);


        result.Select(x => (x.ActionId, x.Time)).Should().Equal(
            ("out", DateTimeOffset.Parse("2022-01-03 18:00")),
            ("in", DateTimeOffset.Parse("2022-01-04 09:00")),
            ("out", DateTimeOffset.Parse("2022-01-04 18:00")));
    }

    [Fact]
    public void CropAndWeave_NowInsideAllInterval()
    {
        var now = DateTimeOffset.Parse("2022-01-03");
        var subject = new DateIntervalList<PeriodicTask>(
            Option.None,
            Option.None,
            new[]
            {
                new PeriodicTask("in", _ => Task.FromResult(0), CronExpression.Parse("0 9 * * *"))
            }.ToList());
        var result = subject.CropAndWeave(now, TimeSpan.Zero);

        result.Select(x => x.Time).Take(5).Should().Equal(
            DateTimeOffset.Parse("2022-01-03 09:00"),
            DateTimeOffset.Parse("2022-01-04 09:00"),
            DateTimeOffset.Parse("2022-01-05 09:00"),
            DateTimeOffset.Parse("2022-01-06 09:00"),
            DateTimeOffset.Parse("2022-01-07 09:00"));
    }

    [Fact]
    public void CropAndWeave_NowInsideFromInterval()
    {
        var now = DateTimeOffset.Parse("2022-01-03");
        var subject = new DateIntervalList<PeriodicTask>(
            DateOnly.Parse("2022-01-01"),
            Option.None,
            new[]
            {
                new PeriodicTask("in", _ => Task.FromResult(0), CronExpression.Parse("0 9 * * *"))
            }.ToList());
        var result = subject.CropAndWeave(now, TimeSpan.Zero);

        result.Select(x => x.Time).Take(5).Should().Equal(
            DateTimeOffset.Parse("2022-01-03 09:00"),
            DateTimeOffset.Parse("2022-01-04 09:00"),
            DateTimeOffset.Parse("2022-01-05 09:00"),
            DateTimeOffset.Parse("2022-01-06 09:00"),
            DateTimeOffset.Parse("2022-01-07 09:00"));
    }

    [Fact]
    public void CropAndWeave_NowInsideToInterval()
    {
        var now = DateTimeOffset.Parse("2022-01-03");
        var subject = new DateIntervalList<PeriodicTask>(
            Option.None,
            DateOnly.Parse("2022-01-04").AddDays(1),
            new[]
            {
                new PeriodicTask("in", _ => Task.FromResult(0), CronExpression.Parse("0 9 * * *"))
            }.ToList());
        var result = subject.CropAndWeave(now, TimeSpan.Zero);

        result.Select(x => x.Time).Should().Equal(
            DateTimeOffset.Parse("2022-01-03 09:00"),
            DateTimeOffset.Parse("2022-01-04 09:00"));
    }
    
    [Fact]
    public void CropAndWeave_NowBeforeInterval()
    {
        var now = DateTimeOffset.Parse("2021-12-31");
        var subject = new DateIntervalList<PeriodicTask>(
            DateOnly.Parse("2022-01-01"),
            DateOnly.Parse("2022-01-04").AddDays(1),
            new[]
            {
                new PeriodicTask("in", _ => Task.FromResult(0), CronExpression.Parse("0 9 * * *"))
            }.ToList());
        var result = subject.CropAndWeave(now, TimeSpan.Zero);

        result.Select(x => x.Time).Take(4).ToList().Should().Equal(
            DateTimeOffset.Parse("2022-01-01 09:00"),
            DateTimeOffset.Parse("2022-01-02 09:00"),
            DateTimeOffset.Parse("2022-01-03 09:00"),
            DateTimeOffset.Parse("2022-01-04 09:00"));
    }

    [Fact]
    public void CropAndWeave_NowAfterInterval()
    {
        var now = DateTimeOffset.Parse("2022-01-06");
        var subject = new DateIntervalList<PeriodicTask>(
            DateOnly.Parse("2022-01-01"),
            DateOnly.Parse("2022-01-04").AddDays(1),
            new[]
            {
                new PeriodicTask("in", _ => Task.FromResult(0), CronExpression.Parse("0 9 * * *"))
            }.ToList());
        var result = subject.CropAndWeave(now, TimeSpan.Zero);

        result.Should().BeEmpty();
    }
}
