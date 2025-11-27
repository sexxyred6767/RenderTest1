using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

// Create the WebApplication
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Get the port from environment variable (Render sets PORT)
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";

// Bind the app to the port
app.Urls.Add($"http://*:{port}");

// Memory store (Render will keep it alive as long as the server runs)
var clients = new Dictionary<string, string>();

// Device registers itself
// POST /register?device=PC1
app.MapPost("/register", (string device) =>
{
    clients[device] = "";
    return Results.Ok($"Registered {device}");
});

// Laptop sends a command to a device
// POST /send?device=PC1&cmd=dir
app.MapPost("/send", (string device, string cmd) =>
{
    clients[device] = cmd;
    return Results.Ok("Command sent");
});

// Device fetches pending command
// GET /recv?device=PC1
app.MapGet("/recv", (string device) =>
{
    if (clients.TryGetValue(device, out var cmd))
    {
        clients[device] = ""; // Clear after receiving
        return Results.Ok(cmd);
    }
    return Results.Ok("");
});

// Start the app
app.Run();
