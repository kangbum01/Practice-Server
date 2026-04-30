using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    public class GameRoom
    {
        private readonly object _lock;
        private readonly List<ClientSession> _clients;

        public string Name { get; private set; }

        public GameRoom(string name)
        {
            Name = name;
            _lock = new object();
            _clients = new List<ClientSession>();
        }

        public void AddClient(ClientSession client)
        {
            lock (_lock)
            {
                // 없는 유저라면
                if (_clients.Contains(client) == false)
                {
                    _clients.Add(client);
                }
            }
        }

        public void RemoveClient(ClientSession client)
        {
            lock (_lock)
            {
                _clients.Remove(client);
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