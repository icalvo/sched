using Cronos;
using Scheduler.Intervals;
using Scheduler.Options;

namespace Scheduler.ConfigParser;

using static Option;

public abstract class ConfigLine
{
    public static ConfigLine Parse(string line)
    {
        static Option<DateOnly> ParseEndpointDate(string date)
            => date == "..." ? None : Some(DateOnly.Parse(date));
        string[] split;

        if (line.StartsWith("R "))
        {
            return new RandomizeConfig(TimeSpan.Parse(line[2..]));
        }

        if (line.StartsWith("+"))
        {
            split = line[1..].Split(" ", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return new AliasConfig(split[0], split[1]);
        }

        split = line.Split(" ", 6, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (line.StartsWith("-") || split[0] == "..." || DateOnly.TryParse(split[0], out _))
        {
            if (line.StartsWith("-"))
            {
                line = line[1..];
            }

            split = line.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return split.Length switch
            {
                0 => new IntervalHeader(Interval.All<DateOnly>()),
                1 =>
                    new IntervalHeader(
                        Interval.Create(
                            ParseEndpointDate(split[0]),
                            ParseEndpointDate(split[0]).Map(x => x.AddDays(1)))),
                2 =>
                    new IntervalHeader(
                        Interval.Create(
                            ParseEndpointDate(split[0]),
                            ParseEndpointDate(split[1]).Map(x => x.AddDays(1)))),
                _ => throw new ArgumentException($"Too many parts in \"{line}\"", nameof(line))
            };
        }

        var cronSplit = split[..5].Reverse().StringJoin(" ");
        var cronExpression = CronExpression.Parse(cronSplit);
        return new TaskConfig(split[5], cronExpression);
    }    
}
