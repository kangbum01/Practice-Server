using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static string _myName = "UnKnown";

        // 게임 상태 및 보드 랜더링용 변수들
        private static bool _isPlaying = false; // 게임 진행 여부 파악
        private static int _myPosX = -1, _myPosY = -1; // 내 좌표
        private static int _oppPosX = -1, _oppPosY = -1; // 상대방 좌표
        private static string _opponentName = ""; // 상대방 이름

        // 오른쪽에 띄워줄 최근 채팅 로그 리스트 (최대 15개)
        private static List<string> _chatLogs = new List<string>();

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            TcpClient client = new TcpClient();
            // 비동기 접속 ConnectAsync(IP, Port)
            await client.ConnectAsync("127.0.0.1", 5000);
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            Console.WriteLine("Connected Server");
            Console.WriteLine("/name NickName");
            Console.WriteLine("/join roomName");
            Console.WriteLine("/rooms ");
            Console.WriteLine("/history");
            Console.WriteLine("/quit");

            // 응답 대기
            Task receiveTask = ReceiveLoopAsync(reader);

            while(true)
            {
                string input = Console.ReadLine();

                if(string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                // 방 이동 명령어
                if(input.StartsWith("/join "))
                {
                    string roomName = input.Substring(6).Trim();
                    await writer.WriteLineAsync("JOIN_ROOM|" + roomName);
                    await writer.FlushAsync();
                }
                else if(input == "/rooms")
                {
                    await writer.WriteLineAsync("ROOM_LIST");
                    await writer.FlushAsync();
                }
                else if(input.StartsWith("/name "))
                {
                    // 닉네임 추출
                    string name = input.Substring(6).Trim();
                    await writer.WriteLineAsync("LOGIN|" + name);
                    await writer.FlushAsync();
                }
                else if(input.StartsWith("/move "))
                {
                    string[] splitInput = input.Split(' ');

                    if(splitInput.Length == 3)
                    {
                        string targetX = splitInput[1];
                        string targetY = splitInput[2];

                        // 서버에 새 포맷(MOVE | X | Y)으로 전송
                        await writer.WriteLineAsync($"MOVE|{targetX}|{targetY}");
                        await writer.FlushAsync();
                    }
                    else
                    {
                        Console.WriteLine("[시스템] 형식 오류: /move X Y (예: /move 1 1)");
                    }
                    continue;
                }
                else if(input == "/history")
                {
                    await writer.WriteLineAsync("REQUEST_CHAT_HISTORY");
                    await writer.FlushAsync();
                }
                else if(input == "/quit")
                {
                    await writer.WriteLineAsync("QUIT");
                    await writer.FlushAsync();
                    break;
                }
                else
                {
                    await writer.WriteLineAsync("CHAT|" + input);
                    await writer.FlushAsync();
                }
            }

            client.Close();
            await receiveTask;
        }

        static async Task ReceiveLoopAsync(StreamReader reader)
        {
            try
            {
                while(true)
                {
                    string message = await reader.ReadLineAsync();

                    if (message == null)
                    {
                        break;
                    }

                    HandleServerPacket(message);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Receive Error:" + ex.Message);
            }
        }

        static void HandleServerPacket(string message)
        {
            // 서버에서 메시지를 받으면 패킷 파싱
            string[] parts = message.Split('|');
            string packetType = parts[0];

            // 로그인 로직
            if(packetType == "LOGIN_OK")
            {
                if (parts.Length >=2 )
                {
                    _myName = parts[1];
                    Console.WriteLine("[Login_Success]" + _myName);
                }
                return;
            }

            // 오류 로직
            if (packetType == "ERROR")
            {
                if(parts.Length >= 2)
                {
                    if (parts[1] == "DUPLICATE_NICKNAME")
                    {
                        Console.WriteLine("[ERROR] ALREADY USED NICKNAME");
                    }
                    else if (parts[1] == "INVALIED_NICKNAME")
                    {
                        Console.WriteLine("[ERROR] INCORRECT NICKNAME");
                    }
                    else
                    {
                        Console.WriteLine("[Error]" + parts[1]);    
                    }
                }
                return;
            }

            // 종료 로직
            if (packetType == "QUIT_OK")
            {
                Console.WriteLine("[Server] Disconnect");
                return;
            }

            // =============================
            //            게임 로직
            // =============================
            // 게임 시작
            if (packetType == "GAME_START")
            {
                _isPlaying = true;
                string p1 = parts[1];
                string p2 = parts[2];

                // 사용자가 p1인지 p2인지 판별하여 초기 위치 설정
                if (_myName == p1)
                {
                    _myPosX = 0; _myPosY = 0;
                    _oppPosX = 4; _oppPosY = 4;
                    _opponentName = p2;
                }
                else
                {
                    _myPosX = 4; _myPosY = 4;
                    _oppPosX = 0; _oppPosY = 0;
                    _opponentName = p1;
                }
                _chatLogs.Add("[시스템] 게임이 시작되었습니다!");
                RenderScreen(); // 화면 갱신
                return;
            }

            // 플레이어 이동(PLAYER_MOVE | NickName | X | Y)
            if (packetType == "PLAYER_MOVE")
            {
                string moveNick = parts[1];
                // 패킷은 string 형태로 전달 되기 때문에 파싱을 해야함
                int newX = int.Parse(parts[2]);
                int newY = int.Parse(parts[3]);

                if (moveNick == _myName)
                {
                    _myPosX = newX; _myPosY = newY;
                }
                else
                {
                    _oppPosX = newX; _oppPosY = newY;
                }

                RenderScreen(); // 좌표가 변경되었기 때문에 화명 갱신
                return;
            }

            // 게임 종료 패킷 수신 (GAME_OVER | WIN | NickName)
            if(packetType == "GAME_OVER" && parts[1] == "WIN")
            {
                _isPlaying = false; // 게임 종료
                string winName = parts[2];

                _chatLogs.Add($"[시스템] 게임 종료! 승리자: {winName}");
                RenderScreen();
                return;
            }

            // 패킷 수신 처리
            if (packetType == "CHAT" || packetType == "SYSTEM")
            {
                // SYSTEM | NickName | Message
                // CHAT | Room Name | NickName | Message
                string logMessage = message.Replace("|", " ");
                _chatLogs.Add(logMessage);

                // 로그가 15줄을 넘어가면 제일 오래된 것 삭제 (화면 안 깨지게)
                if (_chatLogs.Count > 15)
                {
                    _chatLogs.RemoveAt(0);
                }

                RenderScreen(); // 채팅이 추가됐으니 화면 갱신
                return;
            }



            Console.WriteLine("[Server Message]" + message);
        }
        private static void RenderScreen()
        {
            // 1. 화면을 깨끗하게 지웁니다.
            Console.Clear();

            // 게임 중이 아니라면 (대기실 또는 로비) 채팅만 출력
            if (!_isPlaying)
            {
                Console.WriteLine("==== 대기실 (Lobby) 또는 방 대기중 ====");
                foreach (var chat in _chatLogs)
                {
                    Console.WriteLine(chat);
                }
                Console.Write("\n 명령어 입력: ");
                return;
            }

            // 2. [왼쪽 구역] 5 x 5 보드판 그리기 (좌표:0 , 0 부터 시작)
            Console.SetCursorPosition(0,0);
            Console.WriteLine($"==== 5x5 보드 게임 (VS {_opponentName}) ====");

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x ++)
                {
                    if (x == _myPosX && y == _myPosY)
                    {
                        Console.Write("[나]");
                    }
                    else if (x == _oppPosX && y == _oppPosY)
                    {
                        Console.Write("[적]");
                    }
                    else
                    {
                        Console.Write("[ ]");
                    }
                }
                Console.WriteLine();
            }

            // 3. [오른쪽 구역] 채팅 로그 출력 
            int chatRow = 0;
            Console.SetCursorPosition(30, chatRow++);
            Console.WriteLine("=== 실시간 채팅 ===");

            foreach (var chat in _chatLogs)
            {
                Console.SetCursorPosition(30, chatRow++);
                Console.WriteLine(chat);
            }

            // 4. [아래쪽 구역] 입력창 고정
            Console.SetCursorPosition(0,20);
            Console.Write("명령어 입력 (이동: /move x y | 채팅: /chat 메세지):");
            Console.SetCursorPosition(54,20);
        }
    }
}