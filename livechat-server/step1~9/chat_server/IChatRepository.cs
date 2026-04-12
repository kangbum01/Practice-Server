using System.Collections.Generic;

namespace Server
{
    public interface IChatRepository
    {
        //채팅 저장
        void SaveChatMessage(ChatMessage chatMessage);
        // 최근 채팅 가져오기
        List<ChatMessage> GetRecentMessages(int count);
    }
}