using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    public class ServerManager
    {
        private List<ClientSession> _sessions;

        private HashSet<string> _loggedInNickNames;
        private object _sessionLock;
        private IChatRepository _chatRepository;

        private const int MAX_CHAT_HISTORY = 10;

        public RoomManager RoomManager { get; private set; }

        // 저장소를 인자로 받는 ServerManager
        public ServerManager(IChatRepository chatRepository)
        {
            _sessions = new List<ClientSession>();
            _sessionLock = new  object();
            _loggedInNickNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            _chatRepository = chatRepository;

            RoomManager = new RoomManager();
        }
        public void AddSession(ClientSession session)
        {
            lock(_sessionLock)
            {
                _sessions.Add(session);
            }
        }

        public void RemoveSession(ClientSession session)
        {
            lock(_sessionLock)
            {
                _sessions.Remove(session);    
            }
        }

        public bool TryRegisterNickName(string nickName)
        {
            lock(_sessionLock)
            {
                if(_loggedInNickNames.Contains(nickName))
                {
                    return false;
                }
                _loggedInNickNames.Add(nickName);
                return true;
            }
        }

        public void UnregisterNickName(string NickName)
        {
            lock(_sessionLock)
            {
                _loggedInNickNames.Remove(NickName);
            }
        }
        public async Task BroadcastAsync(string message)
        {
            List<ClientSession> copiedSessions;
            lock(_sessionLock)
            {
                copiedSessions = new List<ClientSession>(_sessions);
            }
            foreach (ClientSession session in copiedSessions)
            {
                await session.SendAsync(message);
            }
        }

        // 메시지 저장 인터페이스에 선언된 함수를 사용
        public void SaveChatMessage(ChatMessage chatMessage)
        {
            _chatRepository.SaveChatMessage(chatMessage);
        }
        public List<ChatMessage> GetRecentMessage()
        {
            return _chatRepository.GetRecentMessages(MAX_CHAT_HISTORY);
        }
    }
}