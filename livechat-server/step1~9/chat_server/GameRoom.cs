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

        public bool IsPlayer2Ready { get; private set; } = false;

        // 턴 관리 및 플레이어 할당용 변수
        public ClientSession? Player1 { get; private set; }
        public ClientSession? Player2 { get; private set; }
        public ClientSession? CurrentTurn { get; private set; } 
        public GameRoom(string name)
        {
            Name = name;
            _lock = new object();
            _clients = new List<ClientSession>();
            State = RoomState.WAITING; // 게임 방의 초기 상태는 대기 중
        }

        // 기존 AddClient -> TryAddClient로 변경
        // Player2가 ready를 하고 Player1이 Start를 해야 PLAYING으로 변경
        // 그래서 기존의 코드 삭제함
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
                return true;
            }
        }

        public async Task<bool> RemoveClientAsync(ClientSession client)
        {
            bool wasPlaying = false;
            bool hostChanged = false;
            bool shouldDestroyRoom = false;
            string newHostName = "";

            lock (_lock)
            {
                _clients.Remove(client);

                if (State == RoomState.PLAYING)
                {
                    State = RoomState.WAITING;
                    CurrentTurn = null;
                    wasPlaying = true;
                }

                if (Name != "Lobby" && _clients.Count == 0)
                {
                    shouldDestroyRoom = true;
                }
                else if (Name != "Lobby")
                {
                    // 나간 유저 확인하기
                    if (Player1 == client) 
                    {
                        Player1 = Player2;
                        Player2 = null;
                        newHostName = Player1.NickName;
                        hostChanged = true;
                    }
                    if (Player2 == client)
                    {
                        Player2 = null;
                    }
                }
                IsPlayer2Ready = false;
            }

            if (shouldDestroyRoom) return true;
            if(wasPlaying)
            {
                await BroadcastAsync("GAME_STOPPED|상대방이 퇴장하여 게임이 중단되었습니다.");
            }
            if(hostChanged)
            {
                await BroadcastAsync($"SYSTEM|[시스템] 기존 방장이 퇴장하여 {newHostName}님이 새로운 방장입니다.");
            }

            return false;
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
        
        // Player2가 /ready입력 시
        public async Task HandleReadyAsync(ClientSession client)
        {
            lock (_lock)
            {
                if (State == RoomState.PLAYING) return;

                if (client == Player2)
                {
                    IsPlayer2Ready = !IsPlayer2Ready;
                }
            }
            
            if (client == Player2)
            {
                string statusMsg = IsPlayer2Ready ? "준비 완료! " : "준비 취소";
                await BroadcastAsync($"SYSTEM|[시스템] Player2가 {statusMsg} 상태가 되었습니다.");
            }
            else if (client == Player1)
            {
                await client.SendAsync("SYSTEM|[시스템] 방장(Player1)은 준비 대신 /start를 입력하세요.");
            }
        }

        // Player1이 /start를 쳤을 때
        public async Task HandleStartAsync(ClientSession client)
        {
            bool canStart = false;
            lock (_lock)
            {
                if (State == RoomState.PLAYING) return;

                if (client != Player1) return;
                
                // 여기서 SendAsync를 하지 않는 이유는 lock 안에서 비동기 처리를 해버리면 Deadlock 위험이 있기 때문
                if (_clients.Count < MAX_PLAYERS) return;

                if (IsPlayer2Ready) // Player2가 ready일 때
                {
                    State = RoomState.PLAYING;
                    CurrentTurn = Player1;
                    Player1.SetPosition(0,0);
                    Player2.SetPosition(4,4);
                    canStart = true;
                }
            }

            if(canStart)
            {
                // 서버에 전달
                await BroadcastAsync($"GAME_START|{Player1.NickName}|{Player2.NickName}");
            }
            else if(client == Player1)
            {
                if (_clients.Count < MAX_PLAYERS)
                    await client.SendAsync("SYSTEM|[시스템] 인원이 부족합니다. (2명 필요)");
                else if (!IsPlayer2Ready)
                    await client.SendAsync("SYSTEM|[시스템] Player2가 아직 준비되지 않았습니다.");
            }
        }
    }
}