namespace StudentGroupsHub.Services;

public class EventReminderService(
    IServiceScopeFactory scopeFactory,
    ILogger<EventReminderService> logger) : BackgroundService
{
    private const string Kind = "event_today";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        do
        {
            try
            {
                await NotifyTodaysAttendeesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EventReminderService iteration failed.");
            }
        } while (!stoppingToken.IsCancellationRequested
            && await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task NotifyTodaysAttendeesAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<EventService>();
        var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

        var events = await eventService.GetEventsHappeningTodayAsync();
        foreach (var ev in events)
        {
            ct.ThrowIfCancellationRequested();
            var rsvps = await eventService.GetRsvpsAsync(ev.Id);
            foreach (var rsvp in rsvps.Where(r => r.Status == "going"))
            {
                if (await notificationService.ExistsAsync(rsvp.UserId, Kind, ev.Id)) continue;
                await notificationService.CreateAsync(rsvp.UserId, Kind,
                    $"Hoy es {ev.Title}",
                    $"Tu evento confirmado es hoy a las {ev.StartAt:HH:mm} UTC.",
                    $"/events/{ev.Id}",
                    ev.Id, "event");
            }
        }
    }
}
