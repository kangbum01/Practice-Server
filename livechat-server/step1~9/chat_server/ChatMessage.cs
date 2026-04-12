using System;

namespace Server
{
    public class ChatMessage
    {
        public string NickName { get; private set; }
        public string Message { get; private set; }
        public string SentTime {get; private set; }

        public ChatMessage(string nickName, string message) 
        {
            NickName = nickName;
            Message = message;
            SentTime = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss");
        }
        public string ToChatPacket()
        {
            return "CHAT|" + NickName + "|" + Message + "|" + SentTime;
        }

        public string ToHistoryPacket()
        {
            return "CHAT_HISTORY|" + NickName + "|" + Message + "|" + SentTime;
        }
    }
}

