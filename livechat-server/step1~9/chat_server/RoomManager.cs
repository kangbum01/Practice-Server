using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    public class RoomManager
    {
        private readonly object _lock;
        private readonly Dictionary<string, GameRoom> _rooms;

        public RoomManager()
        {
            _lock = new object();
            //Dictonary는 key - value 형태
            _rooms = new Dictionary<string, GameRoom>();

            GetOrCreateRoom("Lobby");
        }

        // 룸 리스트를 클라이언트에게 전달
        public string BuildRoomListPacket()
        {
            List<string> roomNames = GetRoomNames();
            return "ROOM_LIST|" + string.Join("," , roomNames);
        }

        public GameRoom GetOrCreateRoom(string roomName)
        {
            lock (_lock)
            {
                GameRoom room;

                //roomName의 값을 room에 넣는다 근데 이게 false라는 건 
                // 해당 룸이 없다는 것
                if (_rooms.TryGetValue(roomName, out room) == false)
                {
                    room = new GameRoom(roomName);
                    _rooms.Add(roomName, room);
                }

                return room;
            }
        }

        public List<string> GetRoomNames()
        {
            lock (_lock)
            {
                return new List<string>(_rooms.Keys);
            }
        }

        public async Task MoveClientToRoomAsync(ClientSession client, string roomName)
        {
            GameRoom oldRoom = client.CurrentRoom;
            GameRoom newRoom = GetOrCreateRoom(roomName);

            // 방 입장 시도(꽉 찼는지 확인)
            if (newRoom.TryAddClient(client) == false)
            {
                await client.SendAsync("ERROR|ROOM_FULL");
                return;
            }

            // 기존 방에서 퇴장 처리
            // 방에서 사용자를 제거하고 메세지를 그 방의 유저들에게 전달
            if (oldRoom != null)
            {
                oldRoom.RemoveClient(client);
                await oldRoom.BroadcastAsync("SYSTEM|" + client.NickName + "|Leave");
            }

            client.CurrentRoom = newRoom;
            // client 화면에 현재 이동한 방을 출력
            // 신규 client가 들어간 방의 유저들에게 그 사실을 알린다.
            await client.SendAsync("SYSTEM|Current Room: |" + newRoom.Name);
            await newRoom.BroadcastAsync("SYSTEM|" + client.NickName + "|Enter the room");
        
            // 만약 내가 들어감으로써 방이 꽉 찼다면, 게임 시작 패킷을 전달
            if (newRoom.State == RoomState.PLAYING)
            {
                await newRoom.BroadcastAsync("GAME_START");
            }
        }

        // 기존에는 서버 목록에서만 종료한 유저를 제거하면 됐다.
        // 지금은 룸에서도 유저를 제거해야 하기 때문에 해당 코드가 필요하다
        // 제거할려 하는 유저의 CurrentRoom이 있다면 Room에서 유저를 제거
        public async Task RemoveClientAsync(ClientSession client)
        {
            GameRoom room = client.CurrentRoom;

            if (room != null)
            {
                room.RemoveClient(client);
                await room.BroadcastAsync("SYSTEM|" + client.NickName + "|Leave");
                client.CurrentRoom = null;
            }
        }
    }
}