namespace Scheduler.ConfigParser;

public class RandomizeConfig : ConfigLine
{
    public RandomizeConfig(TimeSpan variation)
    {
        Variation = variation;
    }

    public TimeSpan Variation { get; }
}
