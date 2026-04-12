using System.Collections.Generic;

namespace Server
{
    public class MemoryChatRepository : IChatRepository
    {
        private List<ChatMessage> _message;
        private int _maxHistoryCount;
        private object _lockObject;

        public MemoryChatRepository(int maxHistoryCount)
        {
            _message = new List<ChatMessage>();
            _maxHistoryCount = maxHistoryCount;
            _lockObject = new object();
        }
        public void SaveChatMessage(ChatMessage chatMessage)
        {
            lock(_lockObject)
            {
               _message.Add(chatMessage); 

               if (_message.Count > _maxHistoryCount)
                {
                    _message.RemoveAt(0);
                }
            }
        }
        public List<ChatMessage> GetRecentMessages(int count)
        {
            lock(_lockObject)
            {
                if(count >= _message.Count)
                {
                    return new List<ChatMessage>(_message);
                }
                int startIndex = _message.Count - count;
                return _message.GetRange(startIndex, count);
            }
            
        }
    }
}