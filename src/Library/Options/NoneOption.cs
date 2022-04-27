namespace Scheduler.Options;

public sealed class NoneOption : Option
{
    public override bool IsSome => false;

    public override string ToString()
    {
        return "None";
    }

    public override bool Equals(object? obj)
    {
        return obj?.GetType() == this.GetType();
    }

    public override int GetHashCode() => 0;
}