// 세션에서 채팅을 받으면 1. 저장하고, 2. 패킷으로 브로드캐스트하고, 3. 요청 시 최근 기록을 내린다.
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

        public GameRoom CurrentRoom { get; set; }

        public string NickName { get; private set; }
        public int PosX {get; private set; }
        public int PosY {get; private set; }

        public bool IsLoggedIn {get; private set; }

        // 세션은 하나의 개념, 개발자는 그걸 객체로 만들어서 관리
        public ClientSession(TcpClient client, ServerManager server)
        {
            _client = client;
            _server = server;

            NetworkStream stream = _client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);

            NickName = "Unknown";
            PosX = 0;
            PosY = 0;
        }
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

                    // 메시지 관리
                    await HandleMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== Server Exception Error ===");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                bool wasLoggedIn = IsLoggedIn;
                string LeaveUserNickName = NickName;

                // 일단 Room에서 현재 유저를 제거
                await _server.RoomManager.RemoveClientAsync(this);
                
                _server.RemoveSession(this);
                _client.Close();

                if(wasLoggedIn)
                {
                    //할당된 유저 이름 제거
                    _server.UnregisterNickName(LeaveUserNickName);
                }
            }
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
                    await SendAsync("ERROR|LOGIN_FORMAT");
                    return;
                }

                if(IsLoggedIn)
                {
                    await SendAsync("ERROR|ALREADY_LOGIN");
                    return;
                }
                string requestedNickName = parts[1].Trim();
                if(string.IsNullOrWhiteSpace(requestedNickName))
                {
                    await SendAsync("ERROR|INVALID_NICKNAME");
                    return;
                }
                bool success = _server.TryRegisterNickName(requestedNickName);

                if(!success)
                {
                    await SendAsync("ERROR|DUPLICATE_NICKNAME");
                    return;
                }

                NickName = requestedNickName;
                IsLoggedIn = true;
                await SendAsync("LOGIN_OK|" + NickName);
                // 해당 방의 유저들에게만 새로운 유저의 참가를 알린다
                await _server.RoomManager.MoveClientToRoomAsync(this, "Lobby");
                
                await SendChatHistoryAsync();
                return;
            }

            if(!IsLoggedIn)
            {
                // SendAsync는 서버에 보낼 내용
                // _server.BraodcastAsync는 다른 사용자에게 보낼 내용
                await SendAsync("ERROR|LOGIN_REQUIRED");
                return;
            }

            if (command == "ROOM_LIST")
            {
                await SendAsync(_server.RoomManager.BuildRoomListPacket());
                return;
            }

            if(command == "JOIN_ROOM")
            {
                if (parts.Length < 2)
                {
                    await SendAsync("ERROR|JOIN_ROOM_ERROR");
                    return;
                }

                string roomName = parts[1].Trim();

                if(string.IsNullOrWhiteSpace(roomName))
                {
                    await SendAsync("ERROR|INVALID_ROOM_NAME");
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
                    await SendAsync("ERROR|CHAT_FORMAT");
                    return;
                }

                if (CurrentRoom == null)
                {
                    await SendAsync("ERROR|ROOM_REQUIRED");
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
            if(command == "MOVE")
            {
                if (parts.Length< 2)
                {
                    await SendAsync("ERROR|MOVE_FORMAT");
                    return;
                }

                if (CurrentRoom == null)
                {
                    await SendAsync("ERROR|ROOM_REQUIRED");
                    return;
                }

                string direction = parts[1].ToUpper();
                Move(direction);

                // 사용자 이동 패킷 생성
                string packet = BuildPlayerMovePacket();
                await _server.BroadcastAsync(packet);
                return;
            }

            
            if(command == "QUIT")
            {
                // 해당 세션을 종료
                await SendAsync("QUIT_OK");
                _client.Close();
                return;
            }

            await SendAsync("ERROR|UNKNOWN_COMMAND");
        }

        // 사용자 동작 함수
        private void Move(string direction)
        {
            if (direction == "LEFT")
            {
                PosX = -1;
            }
            else if (direction == "RIGHT")
            {
                PosX += 1;
            }
            else if (direction == "UP")
            {
                PosY += 1;
            }
            else if (direction == "DOWN")
            {
                PosY -= 1;
            }
        }

        //패킷은 서버가 클라이언트에서 전달할 메시지가 담겨져있다.
        // TASK는 언제 끝날 지 모르는 작업, 해당 작업의 종료를 추적할 수 있다.
        private string BuildPlayerMovePacket()
        {
            return "PLAYER_MOVE|" + NickName + "|" + PosX + "|" + PosY; 
        }        private async Task SendChatHistoryAsync()
        {
            List<ChatMessage> history = _server.GetRecentMessage();

            await SendAsync("CHAT_HISTORY_BEGIN");

            foreach (ChatMessage message in history)
            {
                await SendAsync(message.ToHistoryPacket());
            }
            await SendAsync("CHAT_HISTORY_END");
        }

        public async Task SendAsync(string message)
        {
            await _writer.WriteLineAsync(message);
            await _writer.FlushAsync();
        }
    }
}