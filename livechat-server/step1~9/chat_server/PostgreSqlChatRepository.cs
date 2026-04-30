using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace Server
{
    public class PostgreSqlChatRepository : IChatRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        // connectionString은 Host, Password, Database와 같은 키를 사용
        public PostgreSqlChatRepository(string connectionString)
        {
            _dataSource = NpgsqlDataSource.Create(connectionString);
        }

        public void SaveChatMessage(ChatMessage chatMessage)
        {
            try
            {
                using var connection = _dataSource.OpenConnection();

                using var command = new NpgsqlCommand(
                    @"INSERT INTO chat_messages (nickname, message, sent_at)
                    VALUES (@nickname, @message, @sent_at)", connection);
                command.Parameters.AddWithValue("nickname", chatMessage.NickName);
                command.Parameters.AddWithValue("message", chatMessage.Message);
                command.Parameters.AddWithValue("sent_at", DateTime.Parse(chatMessage.SentTime));
                command.ExecuteNonQuery();                
            }
            catch (Exception ex)
            {
                Console.WriteLine("===DB Save Error===");
                Console.WriteLine(ex.ToString());
                throw;
            }
            
        }
        public List<ChatMessage> GetRecentMessages(int count)
        {
            List<ChatMessage> messages = new List<ChatMessage>();

            using var connection = _dataSource.OpenConnection();

            using var command = new NpgsqlCommand(
                @"SELECT nickname, message, sent_at
                  FROM chat_messages
                  ORDER BY id DESC
                  LIMIT @count", connection);
            command.Parameters.AddWithValue("count", count);

            using var reader = command.ExecuteReader();

            while(reader.Read())
            {
                string nickName = reader.GetString(0);
                string message = reader.GetString(1);
                DateTime sentAt = reader.GetDateTime(2);

                messages.Add(
                    new ChatMessage(
                        nickName,
                        message,
                        sentAt.ToString("yyyy-MM-dd HH:mm:ss")
                    )
                );
            }
            messages.Reverse();
            return messages;
        }
    }
}