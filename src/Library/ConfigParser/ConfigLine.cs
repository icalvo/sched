using Cronos;
using Scheduler.Intervals;
using Scheduler.Options;

namespace Scheduler.ConfigParser;

using static Option;

public abstract class ConfigLine
{
    public static ConfigLine Parse(string line)
    {
        static Option<DateOnly> ParseDate(string date)
            => date == "..." ? None : Some(DateOnly.Parse(date));

        if (line.StartsWith("+"))
        {
            var split = line[1..].Split(" ", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return new AliasConfig(split[0], split[1]);
        }
        
        if (line.StartsWith("-"))
        {
            var split = line[1..].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return split.Length switch
            {
                0 => new IntervalHeader(Interval.All<DateOnly>()),
                1 =>
                    new IntervalHeader(
                        Interval.Create(
                            ParseDate(split[0]),
                            ParseDate(split[0]).Map(x => x.AddDays(1)))),
                2 =>
                    new IntervalHeader(
                        Interval.Create(
                            ParseDate(split[0]),
                            ParseDate(split[1]).Map(x => x.AddDays(1)))),
                _ => throw new ArgumentException($"Too many parts in \"{line}\"", nameof(line))
            };
        }
        else
        {
            var split = line.Split(" ", 6, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var cronSplit = split[..5].Reverse().StringJoin(" ");
            var cronExpression = CronExpression.Parse(cronSplit);
            return new TaskConfig(split[5], cronExpression);
        }
    }    
}