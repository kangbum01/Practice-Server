// 클라이언트 세션 관리
using System.Net.Sockets;
namespace GameSocketServer.Models
{
    public class ClientSession
    {
        public string SessionId {get; set; } = Guid.NewGuid().ToString();
        public string PlayerId {get; set; } = string.Empty;
        public TcpClient Client {get; set; }
        public NetworkStream Stream {get; set; }
        public DateTime ConnectedAt {get; set;}
        public DateTime LastHeartbeat {get; set;}

        public ClientSession(TcpClient client)
        {
            Client = client;
            Stream = client.GetStream();
            ConnectedAt = DateTime.UtcNow;
            LastHeartbeat = DateTime.UtcNow;
        }

        public async Task SendPacketAsync(GamePacket packet)
        {
            var data = packet.Serialize();
            await Stream.WriteAsync(data, 0, data.Length);
        }

        public async Task<GamePacket?> ReceivePacketAsync()
        {
            try
            {
                // 4 바이트 데이터 읽기
                var lengthBuffer = new byte[4];
                var bytesRead = await Stream.ReadAsync(lengthBuffer, 0, 4);

                if (bytesRead == 0) return null;

                var length = BitConverter.ToInt32(lengthBuffer, 0);

                // 실제 데이터 읽기
                var dataBuffer = new byte[length];
                var totalRead = 0;

                while(totalRead < length)
                {
                    bytesRead = await Stream.ReadAsync(dataBuffer, totalRead, length - totalRead);
                    if (bytesRead == 0) return null;
                    totalRead += bytesRead;
                }

                return GamePacket.Deserialize(dataBuffer);
            }
            catch
            {
                return null;
            }
        }
    }
}