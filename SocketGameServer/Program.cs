using GameSocketServer.Services;

var port = Environment.GetEnvironmentVariable("GAME_PORT") ?? "9000";
var server = new GameServer(int.Parse(port));

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    Console.WriteLine("\n[Server] Shutting down...");
    server.Stop();
};

await server.StartAsync();
