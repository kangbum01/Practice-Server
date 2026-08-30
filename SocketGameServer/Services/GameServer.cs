using System.Net;
using System.Net.Sockets;
using System.Threading;
using GameSocketServer.Models;

namespace GameSocketServer.Services
{
    public class GameServer
    {
        private readonly int _port;
        private TcpListener? _listener;
        private readonly Dictionary<string, ClientSession> _sessions = new();
        private readonly object _lock = new();
        private CancellationTokenSource? _cts;

        public GameServer(int port)
        {
            _port = port;
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            _cts = new CancellationTokenSource();

            Console.WriteLine($"[Server] Started on port {_port}");
            Console.WriteLine($"[Server] Waiting for connections...");

            _ = Task.Run(() => HeartbeatCheckLoop(_cts.Token));

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClientAsync(client));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Server] Error accepting client: {ex.Message}");
                }
            }
        }
        private async Task HandleClientAsync(TcpClient client)
        {
            var session = new ClientSession(client);

            Console.WriteLine($"[Server] Client connected: {session.SessionId}");

            try
            {
                while(true)
                {
                    var packet = await session.ReceivePacketAsync();

                    if (packet == null)
                    {
                        Console.WriteLine($"[Server] Client disconnected: {session.SessionId}");
                        break;
                    }

                    await ProcessPacketAsync(session, packet);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Server] Error handling client {session.SessionId}: {ex.Message}");
            }
            finally
            {
                RemoveSession(session);
                client.Close();
            }
        }

        private async Task ProcessPacketAsync(ClientSession session, GamePacket packet)
        {
            switch(packet.Type)
            {
                case PacketType.Login:
                    HandleLogin(session, packet);
                    break;
                
                case PacketType.Chat:
                    await HandleChatAsync(session, packet);
                    break;
                
                case PacketType.Move:
                    await BroadcastAsync(packet, session.SessionId);
                    break;

                case PacketType.Heartbeat:
                    session.LastHeartbeat = DateTime.UtcNow;
                    break;
                
                case PacketType.Logout:
                    Console.WriteLine($"[Server] Player {packet.PlayerId} logged out");
                    break;
            }
        }

        private void HandleLogin(ClientSession session, GamePacket packet)
        {
            session.PlayerId = packet.PlayerId;

            lock(_lock)
            {
                _sessions[session.SessionId] = session;
            }

            Console.WriteLine($"[Server] Player logged in: {packet.PlayerId} (Session: {session.SessionId})");
            Console.WriteLine($"[Server] Total players online: {_sessions.Count}");
        }

        private async Task HandleChatAsync(ClientSession session, GamePacket packet)
        {
            Console.WriteLine($"[Server] Chat from {session.PlayerId}: {packet.Data}");
            await BroadcastAsync(packet, session.SessionId);
        }

        private async Task BroadcastAsync(GamePacket packet, string? excludeSessionId = null)
        {
            List<ClientSession> sessions;

            lock(_lock)
            {
                sessions = _sessions.Values
                    .Where(s => s.SessionId != excludeSessionId)
                    .ToList();
            }

            foreach (var session in sessions)
            {
                try
                {
                    await session.SendPacketAsync(packet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Server] Error broadcasting to {session.SessionId}: {ex.Message}");
                }
            }
        }

        private void RemoveSession(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionId);
            }

            Console.WriteLine($"[Server] Session removed: {session.SessionId}");
            Console.WriteLine($"[Server] Total players online: {_sessions.Count}");
        }

        private async Task HeartbeatCheckLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(10000, token);

                var now = DateTime.UtcNow;
                var timeoutSessions = new List<ClientSession>();

                lock(_lock)
                {
                    timeoutSessions = _sessions.Values.Where(s => (now - s.LastHeartbeat).TotalSeconds > 30).ToList();
                }

                foreach(var session in timeoutSessions)
                {
                    Console.WriteLine($"[Server] Heartbeat timeout : {session.SessionId}");
                    session.Client.Close();
                }
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
            _listener?.Stop();

            lock(_lock)
            {
                foreach (var session in _sessions.Values)
                {
                    session.Client.Close();
                }
                _sessions.Clear();
            }

            Console.WriteLine("[Server] Stopped");
        }
    }
}