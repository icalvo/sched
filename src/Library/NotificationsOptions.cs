namespace Scheduler;

public class NotificationsOptions : INotificationsOptions
{
    public string[] NextEvent { get; set; }
    public string[] ConfigurationChanged { get; set; }
    public string[] ConfigurationChangeError { get; set; }
    public string[] CommandSucceeded { get; set; }
    public string[] CommandFailed { get; set; }
}
