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
            IChatRepository chatRepository = new MemoryChatRepository(10);
            ServerManager serverManager = new ServerManager(chatRepository);

            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
            listener.Start();

            Console.WriteLine("Starting Server. Waitng to 127.0.0.1:5000 ");

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