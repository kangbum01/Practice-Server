// 플레이어의 데이터 관리 서비스

using System.Data.Common;
using GameServer.Models;

namespace GameServer.Services
{
    public class PlayerService
    {
        private readonly List<Player> _players = new();
        private int _nextId = 1;

        public Player CreatePlayer(string name)
        {
            var player = new Player
            {
                Id = _nextId++,
                Name = name,
                Score = 0,
                CreateAt = DateTime.UtcNow
            };
            _players.Add(player);
            return player;
        }

        public Player? GetPlayer(int id)
        {
            return _players.FirstOrDefault(p => p.Id == id);
        }

        public List<Player> GetAllPlayers()
        {
            return _players;
        }

        public bool UpdateScore(int id, int score)
        {
            var player = GetPlayer(id);
            if (player == null) return false;

            player.Score = score;
            return true;
        }
    }
}