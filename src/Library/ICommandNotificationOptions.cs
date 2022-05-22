namespace Scheduler;

public interface ICommandNotificationOptions
{
    string? CommandSucceeded { get; set; }
    string? CommandFailed { get; set; }
}
