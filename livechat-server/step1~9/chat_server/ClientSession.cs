// 세션에서 채팅을 받으면 1. 저장하고, 2. 패킷으로 브로드캐스트하고, 3. 요청 시 최근 기록을 내린다.
// 05/29 ping-pong 로직 추가
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Server
{
    public class ClientSession
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        private ServerManager _server;

        public GameRoom? CurrentRoom { get; set; }

        // 마지막으로 클라이언트의 신호(Ping/Pong)를 받은 시간
        public DateTime LastHeartbeatTime {get; set; } = DateTime.Now;

        public string NickName { get; private set; }
        public int PosX {get; private set; }
        public int PosY {get; private set; }

        public bool IsLoggedIn {get; private set; }

        // 세션은 하나의 개념, 개발자는 그걸 객체로 만들어서 관리
        public ClientSession(TcpClient client, ServerManager server)
        {
            _client = client;
            _server = server;

            // TCP 네이글 알고리즘 비활성화
            // 네이클 알고리즘은 토큰이 가득 차면 전달하는 알고리즘으로
            // 현재 작업에서는 일정 시간이 지나면 자동으로 전달하는 Tick 기반 패킷 배치 시스템이기 때문에
            // 비활성화 해야 함
            _client.NoDelay = true;

            NetworkStream stream = _client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);

            NickName = "Unknown";
            PosX = 0;
            PosY = 0;
        }
        // Ping-Pong 로직 추가
        // 마지막으로 클라이언트의 신호(Ping/Pong 포함)를 받은 시간

        public async Task ProcessAsync()
        {
            try
            {
                while(true)
                {
                    //메지시를 읽어요
                    string message = await _reader.ReadLineAsync();

                    if (message == null)
                    {
                        break;
                    }

                    // 패킷이 들어왔다는건 살아 있는 세션이기 때문에 갱신
                    LastHeartbeatTime = DateTime.Now;

                    string[] parts = message.Split('|');
                    string command = parts[0];

                    // 서버 -> 클라이언트 (PING)
                    // 클라이언트 -> 서버 (PONG)
                    // 그래서 ClientSession은 Client가 PONG을 보내면 아직 살아 있다 판단
                    if (command == "PONG")
                    {
                        continue;
                    }

                    Console.WriteLine($"[RECV | {NickName}] {message}");
                    // 메시지 관리
                    await HandleMessageAsync(message);
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"[DISCONNECT] {NickName}님의 연결이 강제로 끊어졌습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== Server Exception Error ===");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                await CleanUpSessionAsync();
            }
        }


        private async Task CleanUpSessionAsync()
        {
            // 유저가 방에 있었다면 방에서 제거
            if (CurrentRoom != null)
            {
                await _server.RoomManager.RemoveClientAsync(this);
            }

            // 서버 관리 목록에서 제거 및 닉네임 반환
            _server.RemoveSession(this);
            if (NickName != "Unknown")
            {
                _server.UnregisterNickName(NickName);
            }

            // 소켓 스트림 닫기 (메모리 해제)
            _client.Close();

            Console.WriteLine($"[CLEANUP] {NickName}님의 리소스가 성공적으로 정리되었습니다.");
        }

        private async Task HandleMessageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            // | 을 기준으로 분류
            string[] parts = message.Split('|');
            // 받은 내용 대문자로 전환
            string command = parts[0].ToUpper();
            if (command == "LOGIN")
            {
                if (parts.Length < 2)
                {
                    await EnqueuePacketAsync("ERROR|LOGIN_FORMAT");
                    return;
                }

                if(IsLoggedIn)
                {
                    await EnqueuePacketAsync("ERROR|ALREADY_LOGIN");
                    return;
                }
                string requestedNickName = parts[1].Trim();
                if(string.IsNullOrWhiteSpace(requestedNickName))
                {
                    await EnqueuePacketAsync("ERROR|INVALID_NICKNAME");
                    return;
                }
                bool success = _server.TryRegisterNickName(requestedNickName);

                if(!success)
                {
                    await EnqueuePacketAsync("ERROR|DUPLICATE_NICKNAME");
                    return;
                }

                NickName = requestedNickName;
                IsLoggedIn = true;
                await EnqueuePacketAsync("LOGIN_OK|" + NickName);
                // 해당 방의 유저들에게만 새로운 유저의 참가를 알린다
                await _server.RoomManager.MoveClientToRoomAsync(this, "Lobby");
                
                await SendChatHistoryAsync();
                return;
            }

            if(!IsLoggedIn)
            {
                // EnqueuePacketAsync는 서버에 보낼 내용
                // _server.BraodcastAsync는 다른 사용자에게 보낼 내용
                await EnqueuePacketAsync("ERROR|LOGIN_REQUIRED");
                return;
            }

            if (command == "ROOM_LIST")
            {
                await EnqueuePacketAsync(_server.RoomManager.BuildRoomListPacket());
                return;
            }

            if(command == "JOIN_ROOM")
            {
                if (parts.Length < 2)
                {
                    await EnqueuePacketAsync("ERROR|JOIN_ROOM_ERROR");
                    return;
                }

                string roomName = parts[1].Trim();

                if(string.IsNullOrWhiteSpace(roomName))
                {
                    await EnqueuePacketAsync("ERROR|INVALID_ROOM_NAME");
                    return;
                }

                // 출력 예시 : JOIN_ROOM|Room1
                await _server.RoomManager.MoveClientToRoomAsync(this, roomName);
                return;
            }

            if (command == "CHAT")
            {
                if(parts.Length < 2)
                {
                    await EnqueuePacketAsync("ERROR|CHAT_FORMAT");
                    return;
                }

                if (CurrentRoom == null)
                {
                    await EnqueuePacketAsync("ERROR|ROOM_REQUIRED");
                    return;
                }
            
                string chatText = parts[1];

                ChatMessage chatMessage = new ChatMessage(NickName, chatText);
                _server.SaveChatMessage(chatMessage);

                // 메세지 전송을 전체 서버에서 현재 방으로 한정
                await CurrentRoom.BroadcastAsync("CHAT|" + CurrentRoom.Name + "|" + NickName + "|" + chatText);
                return;
            }
            if (command == "REQUEST_CHAT_HISTORY")
            {
                await SendChatHistoryAsync();
                return;
            }

            if (command == "READY")
            {
                if(CurrentRoom != null && CurrentRoom.Name != "Lobby")
                {
                    await CurrentRoom.HandleReadyAsync(this);
                }
                return;
            }

            if (command == "START")
            {
                if(CurrentRoom != null && CurrentRoom.Name != "Lobby")
                {
                    await CurrentRoom.HandleStartAsync(this);
                }
                return;
            }
            if(command == "MOVE")
            {   
                // MOVE|목표X|목표Y 형태로 전달되었는지 확인
                if (parts.Length< 3)
                {
                    await EnqueuePacketAsync("ERROR|MOVE_FORMAT_INVALID");
                    return;
                }

                if (CurrentRoom == null || CurrentRoom.Name == "Lobby")
                {
                    await EnqueuePacketAsync("ERROR|ROOM_REQUIRED");
                    return;
                }

                // 1단계: 게임 상태 및 턴(Turn) 검증
                if (CurrentRoom.State != RoomState.PLAYING)
                {
                    await EnqueuePacketAsync("ERROR|GAME_NOT_STARTED");
                    return;
                }

                if (CurrentRoom.CurrentTurn != this)
                {
                    await EnqueuePacketAsync("ERROR|NOT_YOUR_TURN");
                    return;
                }

                // 2단계 가상 좌표 파싱
                if (!int.TryParse(parts[1], out int targetX) || !int.TryParse(parts[2], out int targetY))
                {
                    await EnqueuePacketAsync("ERROR|INVALID_COORDINATE_FORMAT");
                    return;
                }

                // 3단계 규칙 검증 (맵 이탈 방지 및 1칸 이동 제어)
                // 3-1 5x5 보드판 (0~4) 밖으로 나갔는지 확인
                if (targetX < 0 || targetX > 4 || targetY < 0 || targetY > 4)
                {
                    await EnqueuePacketAsync("ERROR|OUT_OF_BOUNDS");
                    return;
                }
                
                // 3-2 가로, 세로, 대각선으로 딱 1칸만 움직였는지 확인
                int diffX = Math.Abs(targetX - PosX);
                int diffY = Math.Abs(targetY - PosY);

                if (diffX > 1 || diffY > 1 || (diffX == 0 && diffY == 0))
                {
                    await EnqueuePacketAsync("ERROR|INVALID_MOVE_RULE");
                    return;
                }

                // 4단계: 상태 확정 및 승패 판정 및 턴 넘기기

                PosX = targetX;
                PosY = targetY;

                // 내 상대방의 위치 찾기
                ClientSession opponent = (CurrentRoom.Player1 == this) ? CurrentRoom.Player2 : CurrentRoom.Player1;
                
                // 승패 판정
                if (opponent != null && opponent.PosX == PosX && opponent.PosY == PosY)
                {
                    await CurrentRoom.BroadcastAsync("GAME_OVER|WIN|" + NickName);
                }

                else
                {
                    string movePacket = BuildPlayerMovePacket();
                    await CurrentRoom.BroadcastAsync(movePacket);
                    CurrentRoom.SwitchTurn();
                }
                
                return;
            }

            
            if(command == "QUIT")
            {
                // 해당 세션을 종료
                await EnqueuePacketAsync("QUIT_OK");
                _client.Close();
                return;
            }

            await EnqueuePacketAsync("ERROR|UNKNOWN_COMMAND");
        }

        public void SetPosition(int x, int y)
        {
            PosX = x;
            PosY = y;
        }

        //패킷은 서버가 클라이언트에서 전달할 메시지가 담겨져있다.
        // TASK는 언제 끝날 지 모르는 작업, 해당 작업의 종료를 추적할 수 있다.
        private string BuildPlayerMovePacket()
        {
            return "PLAYER_MOVE|" + NickName + "|" + PosX + "|" + PosY; 
        }        
        private async Task SendChatHistoryAsync()
        {
            List<ChatMessage> history = _server.GetRecentMessage();

            await EnqueuePacketAsync("CHAT_HISTORY_BEGIN");

            foreach (ChatMessage message in history)
            {
                await EnqueuePacketAsync(message.ToHistoryPacket());
            }
            await EnqueuePacketAsync("CHAT_HISTORY_END");
        }

        // 기존의 EnqueuePacketAsync를 쪼개는 작업
        // public async Task EnqueuePacketAsync(string message)
        // {
        //     try
        //     {
        //         Console.WriteLine($"[SEND | {NickName}] {message}");

        //         await _writer.WriteLineAsync(message);
        //         await _writer.FlushAsync();
        //     }
        //     catch(ObjectDisposedException)
        //     {
                
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"[SEND ERROR | {NickName}] 패킷 전송 실패: {ex.Message}");
        //     }
        // }

        // 1. EnqueuePacketAsync는 버퍼에 쌓는 메서드
        public async Task EnqueuePacketAsync(string message)
        {
            try
            {
                Console.WriteLine($"[ENQUEUE | {NickName}] {message}");
                await _writer.WriteLineAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ENQUEUE ERROR | {NickName}] 패킷 버퍼링 실패: {ex.Message}");
            }
        }
        // 2. FlushNetworkAsync()는 전송 메서드
        public async Task FlushNetworkAsync()
        {
            try
            {
                await _writer.FlushAsync();
            }
            catch (ObjectDisposedException)
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FLUSH ERROR | {NickName}] 패킷 전송 실패: {ex.Message}");
            }
        }
    }
}