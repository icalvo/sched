namespace Scheduler.Options;

public static class OptionExtensions
{
    public static Option<T> StructToOption<T>(this T? nullable) where T : struct
        => nullable.HasValue ? Option.Some(nullable.Value) : Option.None;

    public static Option<T> ClassToOption<T>(this T? nullable) where T : class
        => nullable == null ? Option.None : Option.Some(nullable);
}