using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 전과 달리 postgresql에 데이터를 저장
            string connectionString = "Host=127.0.0.1;Port=5432;Username=postgres;Password=qwer1234;Database=postgres";   
            //저장소를 넣어줄 ServerManager
            
            IChatRepository chatRepository = new PostgreSqlChatRepository(connectionString);
            ServerManager serverManager = new ServerManager(chatRepository);

            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
            listener.Start(10000);

            // Console.WriteLine("Starting Server. Waitng to 127.0.0.1:5000 ");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Client Connect.");

                ClientSession session = new ClientSession(client, serverManager);
                serverManager.AddSession(session);

                _ = session.ProcessAsync();
            }
        }
    }
}