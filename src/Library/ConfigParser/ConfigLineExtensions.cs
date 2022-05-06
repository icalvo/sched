using System.Collections.Immutable;
using Scheduler.Intervals;
using Scheduler.Options;

namespace Scheduler.ConfigParser;

using static Option;
public static class ConfigLineExtensions
{
    public static IEnumerable<DateIntervalList<PeriodicTask>> AggregateScheduleGroups(
        this IEnumerable<ConfigLine> source,
        IActionParser parser)
    {
        var config = source
            .Aggregate(
                new SchedulerConfiguration(),
                (accum, parsedLine) =>
                {
                    var (sg, ac) = accum;
                    return parsedLine switch
                    {
                        AliasConfig c => accum with { Aliases = ac.Add(c.Alias, c.CommandLine) },
                        TaskConfig t =>
                            accum with
                            {
                                Groups = (ImmutableList<DateIntervalList<TaskConfig>>)(
                                    sg.IsEmpty
                                        ? sg.Add(new DateIntervalList<TaskConfig>(None, None, new[] { t }.ToList()))
                                        : sg.UpdateLast(x => x.Element.Add(t)))
                            },
                        IntervalHeader dl =>
                            accum with
                            {
                                Groups = sg.Add(new DateIntervalList<TaskConfig>(dl.Interval))
                            },
                        _ => accum
                    };
                });

        PeriodicTask Convert(TaskConfig taskConfig) => parser.Convert(taskConfig, config.Aliases);

        return config.Groups.Select(x =>
            new DateIntervalList<PeriodicTask>(x.Start, x.End, x.Element.Select(Convert).ToList()));
    }

    private static IImmutableList<T> UpdateLast<T>(this IImmutableList<T> list, Action<T> updater)
    {
        var last = list[^1];
        updater(last);
        return list;
    }
}
