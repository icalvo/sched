namespace Scheduler;

public interface INotificationsOptions : ICommandNotificationOptions
{
    string? NextEvent { get; set; }
    string? ConfigurationChanged { get; set; }
    string? ConfigurationChangeError { get; set; }
}
