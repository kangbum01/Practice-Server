using System.Net.Sockets;
using System.Text;
using System.Text.Json;


class GameClient
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly string _host;
    private readonly int _port;
    private readonly string _playerId;

    public GameClient(string host, int port, string playerId)
    {
        _host = host;
        _port = port;
        _playerId = playerId;
    }

    public async Task ConnectAsync()
    {
        _client = new TcpClient();
        await _client.ConnectAsync(_host, _port);
        _stream = _client.GetStream();

        Console.WriteLine($"Connect to {_host}:{_port}");
        

        // 로그인 패킷 전달
        await SendLoginAsync();

        // 수신 루프 시작
        _ = Task.Run(ReceiveLoop);
    }

    private async Task SendLoginAsync()
    {
        var packet = new
        {
            Type = 1,
            PlayerId = _playerId,
            Data = "",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        await SendPacketAsync(packet);
        Console.WriteLine($"Logged in as {_playerId}");
    }

    public async Task SendChatAsync(string message)
    {
        var packet = new
        {
            Type = 3,
            PlayerId = _playerId,
            Data = message,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        await SendPacketAsync(packet);
    }

    private async Task SendPacketAsync(object packet)
    {
        var json = JsonSerializer.Serialize(packet);
        var bytes = Encoding.UTF8.GetBytes(json);
        var length = BitConverter.GetBytes(bytes.Length);

        var data = new byte[4 + bytes.Length];
        Array.Copy(length, 0, data, 0, 4);
        Array.Copy(bytes, 0, data, 4, bytes.Length);

        await _stream!.WriteAsync(data, 0, data.Length);
    }

    private async Task ReceiveLoop()
    {
        try
        {
            while(true)
            {
                // 길이(4바이트) 읽기 - 부분 읽기 대비
                var lengthBuffer = new byte[4];
                int bytesRead = await _stream!.ReadAsync(lengthBuffer, 0, 4);
                if (bytesRead == 0) throw new Exception("Remote closed");
                int totalLenRead = bytesRead;
                while (totalLenRead < 4)
                {
                    bytesRead = await _stream.ReadAsync(lengthBuffer, totalLenRead, 4 - totalLenRead);
                    if (bytesRead == 0) throw new Exception("Remote closed");
                    totalLenRead += bytesRead;
                }

                var length = BitConverter.ToInt32(lengthBuffer, 0);
                if (length <= 0) continue;

                var dataBuffer = new byte[length];
                var totalRead = 0;

                while(totalRead < length)
                {
                    bytesRead = await _stream.ReadAsync(dataBuffer, totalRead, length - totalRead);
                    if (bytesRead == 0) throw new Exception("Remote closed");
                    totalRead += bytesRead;
                }

                var json = Encoding.UTF8.GetString(dataBuffer);
                var packet = JsonSerializer.Deserialize<JsonElement>(json);

                var type = packet.GetProperty("Type").GetInt32();
                var playerId = packet.GetProperty("PlayerId").GetString();
                var data = packet.GetProperty("Data").GetString();

                if (type == 3) // chat
                {
                    Console.WriteLine($"[{playerId}] : {data}");
                }
            }
        }
        catch( Exception ex)
        {
            Console.WriteLine($"Connection closed: {ex.Message}");
        }
    }
    
    public void Disconnect()
    {
        _stream?.Close();
        _client?.Close();
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var host = args.Length > 0 ? args[0] : "localhost";
        var port = args.Length > 1 ? int.Parse(args[1]) : 9000;
        var playerId = args.Length > 2 ? args[2] : $"Player_{Random.Shared.Next(1000, 9000)}";

        var client = new GameClient(host, port, playerId);

        try
        {
            await client.ConnectAsync();

            Console.WriteLine("Command: /chat <message>, /quit");

            while(true)
            {
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;

                if (input == "/quit")
                {
                    break;
                }

                if (input.StartsWith("/chat "))
                {
                    var message = input.Substring(6);
                    await client.SendChatAsync(message);
                }
            }
        }
        finally
        {
            client.Disconnect();
        }
    }
}
