// 서버 - 클라이언트 메시지 포맷
using System.Text.Json;

namespace GameSocketServer.Models
{
    public enum PacketType
    {
        Login = 1,
        Logout = 2,
        Chat = 3,
        Move = 4,
        Heartbeat = 5
    }

    public class GamePacket
    {
        public PacketType Type { get; set; }
        public string PlayerId {get; set; } = string.Empty;
        public string Data {get; set; } = string.Empty;
        public long Timestamp {get; set; }

        // 직렬화 작업
        public byte[] Serialize()
        {
            var json = JsonSerializer.Serialize(this);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var length = BitConverter.GetBytes(bytes.Length);

            // 4 바이트 길이 + JSON 데이터
            var packet = new byte[4 + bytes.Length];
            Array.Copy(length, 0, packet, 0, 4);
            Array.Copy(bytes, 0, packet, 4, bytes.Length);

            return packet;
        }

        // 역직렬화 작업
        public static GamePacket? Deserialize(byte[] data)
        {
            var json = System.Text.Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<GamePacket>(json);
        }
    }
}