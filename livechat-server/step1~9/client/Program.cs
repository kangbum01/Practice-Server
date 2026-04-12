using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static string _myName = "UnKnown";

        static async Task Main(string[] args)
        {
            TcpClient client = new TcpClient();
            // 비동기 접속 ConnectAsync(IP, Port)
            await client.ConnectAsync("127.0.0.1", 5000);
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            Console.WriteLine("Connected Server");
            Console.WriteLine("/name NickName");
            Console.WriteLine("/move left");
            Console.WriteLine("/move right");
            Console.WriteLine("/move up");
            Console.WriteLine("/move down");
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

                if(input.StartsWith("/name "))
                {
                    // 닉네임 추출
                    string name = input.Substring(6).Trim();
                    await writer.WriteLineAsync("LOGIN|" + name);
                    await writer.FlushAsync();
                }
                else if(input.StartsWith("/move "))
                {
                    string direction = input.Substring(6).Trim().ToUpper();
                    await writer.WriteLineAsync("MOVE|" + direction);
                    await writer.FlushAsync();
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
            string[] parts = message.Split('|');
            string packetType = parts[0];

            if(packetType == "LOGIN_OK")
            {
                if (parts.Length >=2 )
                {
                    _myName = parts[1];
                    Console.WriteLine("[Login_Success]" + _myName);
                }
                return;
            }

            if(packetType == "SYSTEM")
            {
                // [System|NickName|JOIN Or LEAVE]
                if(parts.Length >= 3)
                {
                    string nickName = parts[1];
                    string action = parts[2];

                    if(action == "JOIN")
                    {
                        Console.WriteLine("[SYSTEM]" + nickName + "Join");
                    }
                    else if(action == "LEAVE")
                    {
                        Console.WriteLine("[SYSTEM]"+ nickName + "LEAVE");
                    }
                }
                return;
            }

            if(packetType == "PLAYER_MOVE")
            {
                // 타입|닉네임|Xpos|Ypos
                if(parts.Length >= 4)
                {
                    string nickName = parts[1];
                    string x = parts[2];
                    string y = parts[3];

                    if(nickName == _myName)
                    {
                        Console.WriteLine("[내 이동]" + nickName + "-> ( " + x + ", " + y + ")");         
                    }
                    else
                    {
                        Console.WriteLine("[다른 플레이어 이동]" + nickName + "-> ( " + x + ", " + y + ")");
                    }
                }
                return;
            }
            if (packetType == "CHAT")
            {
                if(parts.Length >= 4)
                {
                    string nickName = parts[1];
                    string chatText = parts[2];
                    string sentTime = parts[3];

                    Console.WriteLine("[CHAT][" + sentTime + "]" + nickName + ":" + chatText);
                }
                return;
            }
            if (packetType == "CHAT_HISTORY_BEGIN")
            {
                Console.WriteLine("====Recently Chat History====");
                return;
            }
            if (packetType == "CHAT_HISTORY")
            {
                if(parts.Length >= 4)
                {
                    string nickName = parts[1];
                    string chatText = parts[2];
                    string sentTime = parts[3];

                    Console.WriteLine("[History][" + sentTime + "]" + nickName + ":" + chatText);
                }
                return;
            }

            if (packetType == "CHAT_HISTORY_END")
            {
                Console.WriteLine("Recently Chat Finish");
                return;
            }

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

            if (packetType == "QUIT_OK")
            {
                Console.WriteLine("[Server] Disconnect");
                return;
            }

            Console.WriteLine("[Server Message]" + message);
        }
    }
}