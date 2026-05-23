using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    // 방의 현재 상태를 정의하는 enum
    public enum RoomState
    {
        WAITING, // 대기 중(1명 이하)
        PLAYING // 게임 중 (2명 꽉 참)
    }
    public class GameRoom
    {
        private readonly object _lock;
        private readonly List<ClientSession> _clients;

        public string Name { get; private set; }

        public RoomState State { get; private set; } // 방 상태
        public const int MAX_PLAYERS = 2; // 최대 인원

        // 턴 관리 및 플레이어 할당용 변수
        public ClientSession Player1 { get; private set; }
        public ClientSession Player2 { get; private set; }
        public ClientSession CurrentTurn { get; private set; } 
        public GameRoom(string name)
        {
            Name = name;
            _lock = new object();
            _clients = new List<ClientSession>();
            State = RoomState.WAITING; // 게임 방의 초기 상태는 대기 중
        }

        // 기존 AddClient -> TryAddClient로 변경
        public bool TryAddClient(ClientSession client)
        {
            lock (_lock)
            {
                // Lobby가 아니면서 방이 이미 꽉 찼다면
                if (Name != "Lobby" && _clients.Count >= MAX_PLAYERS)
                {
                    return false;
                }
                // 없는 유저라면
                if (_clients.Contains(client) == false)
                {
                    _clients.Add(client);

                    // 플레이어 1,2 할당 로직
                    if(Name != "Lobby")
                    {
                        if (Player1 == null)
                        {
                            Player1 = client;
                        }
                        else if (Player2 == null)
                        {
                            Player2 = client;
                        }
                    }
                }

                // 사용자를 포함해서 방 인원이 최대 인원이 되었다면 상태 변경
                if (Name != "Lobby" && _clients.Count == MAX_PLAYERS)
                {
                    State = RoomState.PLAYING;

                    // 시작은 방장(Player1 부터)
                    CurrentTurn = Player1;

                    // 캡슐화 유지를 위해 ClientSessions에 Set함수 추가
                    Player1.SetPosition(0,0);
                    Player2.SetPosition(4,4);
                }

                return true;
            }
        }

        public void RemoveClient(ClientSession client)
        {
            lock (_lock)
            {
                _clients.Remove(client);

                if (Name != "Lobby")
                {
                    // 나간 유저 확인하기
                    if (Player1 == client) 
                    {
                        Player1 = Player2;
                        Player2 = null;
                    }
                    if (Player2 == client) 
                    {
                        Player2 = null;
                    }
                    if(_clients.Count < MAX_PLAYERS)
                    {
                        State = RoomState.WAITING;
                        CurrentTurn = null;
                    }
                }
            }
        }

        // 턴 넘기기 함수
        public void SwitchTurn()
        {
            lock (_lock)
            {
                if (CurrentTurn == Player1)
                {
                    CurrentTurn = Player2;
                }
                else
                {
                    CurrentTurn = Player1;
                }
            }
        }
        public List<ClientSession> GetClientsSnapshot()
        {
            lock (_lock)
            {
                return new List<ClientSession>(_clients);
            }
        }

        // 입장한 본인을 제외하고 기존의 방에 있던 사람들에게 메세지를 보내기 위해 있는 코드
        public async Task BroadcastAsync(string message, ClientSession excludedClient = null)
        {
            List<ClientSession> copiedClients = GetClientsSnapshot();

            foreach (ClientSession client in copiedClients)
            {
                //제외 전송 파트 
                
                if (excludedClient != null && client == excludedClient)
                {
                    continue;
                }

                await client.SendAsync(message);
            }
        }
    }
}