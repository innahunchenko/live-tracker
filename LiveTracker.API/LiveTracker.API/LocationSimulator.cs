using LiveTracker.API;
using Microsoft.AspNetCore.SignalR;

public class LocationSimulator : BackgroundService
{
    private readonly IHubContext<TrackingHub> hub;

    private readonly List<(double lat, double lng)> route = new()
    {
        (50.4501, 30.5234),
        (50.4505, 30.5236),
        (50.4509, 30.5239),
        (50.4513, 30.5242),
        (50.4517, 30.5245),
        (50.4521, 30.5248),
        (50.4525, 30.5252),
        (50.4529, 30.5255),
        (50.4533, 30.5258),
        (50.4537, 30.5262),
    };

    private int index = 0;

    public LocationSimulator(IHubContext<TrackingHub> hub)
    {
        this.hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var point = route[index];

            // Send the current coordinate to all connected clients
            await hub.Clients.All.SendAsync("ReceiveLocation", point.lat, point.lng, cancellationToken: stoppingToken);

            // Move to the next point in the route
            index++;
            if (index >= route.Count)
                index = 0; // loop back to the start, or stop if you want

            // Wait 1 second before sending the next coordinate
            await Task.Delay(1000, stoppingToken);
        }
    }
}