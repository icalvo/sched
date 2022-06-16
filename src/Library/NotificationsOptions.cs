namespace Scheduler;

public class NotificationsOptions : INotificationsOptions
{
    public NotificationsOptions() : this(null)
    {
    }

    public NotificationsOptions(
        string[]? nextEvent = null,
        string[]? configurationChanged = null,
        string[]? configurationChangeError = null,
        string[]? commandSucceeded = null,
        string[]? commandFailed = null)
    {
        NextEvent = nextEvent ?? Array.Empty<string>();
        ConfigurationChanged = configurationChanged ?? Array.Empty<string>();
        ConfigurationChangeError = configurationChangeError ?? Array.Empty<string>();
        CommandSucceeded = commandSucceeded ?? Array.Empty<string>();
        CommandFailed = commandFailed ?? Array.Empty<string>();
    }

    public string[] NextEvent { get; set; }
    public string[] ConfigurationChanged { get; set; }
    public string[] ConfigurationChangeError { get; set; }
    public string[] CommandSucceeded { get; set; }
    public string[] CommandFailed { get; set; }
}
