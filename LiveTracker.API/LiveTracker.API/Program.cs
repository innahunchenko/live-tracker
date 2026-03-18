using LiveTracker.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddHostedService<LocationSimulator>();

var frontendUrl = builder.Configuration["Frontend:Url"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(frontendUrl) 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); 
    });
});

var app = builder.Build();

app.UseCors();

app.MapHub<TrackingHub>("/tracking");

app.Run();