namespace Scheduler.ConfigParser;

public sealed class AliasConfig : ConfigLine
{
    public AliasConfig(string alias, string commandLine)
    {
        Alias = alias;
        CommandLine = commandLine;
    }

    public string Alias { get; }
    public string CommandLine { get; }

    private bool Equals(AliasConfig other)
    {
        return Alias == other.Alias;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((AliasConfig)obj);
    }

    public override int GetHashCode()
    {
        return Alias.GetHashCode();
    }
}