using Scheduler.ConfigParser;
using Scheduler.Intervals;

namespace Scheduler;

public static class EventBuilder
{
    public static (
        List<DateIntervalList<PeriodicTask>> scheduleGroups,
        List<IntervalElement<DateOnly, List<PeriodicTask>>> flattenedGroups,
        IEnumerable<OneTimeTask> events) BuildEvents(IActionParser actionParser, string configPath)
    {
        var now = DateTimeOffset.Now;

        string[]? lines = null;
        for (int i = 0; i < 5; i++)
        {
            try
            {
                lines = File.ReadAllLines(configPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.Sleep(1000);
            }
        }

        if (lines == null) throw new Exception("Cannot read the file");

        var parsedLines = lines
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Where(line => !line.StartsWith("#"))
            .Select(ConfigLine.Parse);

        var config = parsedLines.AggregateScheduleGroups(actionParser);
        var scheduleGroups = config.ToList();
        var flattenedGroups = scheduleGroups.Flatten().ToList();
        var events = flattenedGroups.SelectMany(x => x.CropAndWeave(now));

        return (scheduleGroups, flattenedGroups, events);
    }
}