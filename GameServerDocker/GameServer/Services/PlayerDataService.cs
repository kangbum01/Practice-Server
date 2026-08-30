using System.Text.Json;
using GameServer.Models;

namespace GameServer.Services
{
    public class PlayerDataService
    {
        private readonly string _dataPath;
        private readonly Dictionary<int, Player> _players = new();
        private int _nextId = 1;

        public PlayerDataService(IConfiguration configuration)
        {
            _dataPath = configuration["DataPath"] ?? "/app/data";
            Directory.CreateDirectory(_dataPath);
            LoadPlayers();
        }

        public Player CreatePlayer(string name)
        {
            var player = new Player
            {
                Id = _nextId,
                Name = name,
                Score = 0,
                CreateAt = DateTime.UtcNow
            };

            _players[player.Id] = player;
            SavePlayer(player);

            return player;
        }

        public Player? GetPlayer(int id)
        {
            _players.TryGetValue(id, out var player);
            return player;
        }

        public List<Player> GetAllPlayers()
        {
            return _players.Values.ToList();
        }

        public bool UpdateScore(int id, int score)
        {
            if (!_players.TryGetValue(id, out var player))
                return false;

            player.Score = score;
            SavePlayer(player);

            return true;
        }

        private void SavePlayer(Player player)
        {
            var filePath = Path.Combine(_dataPath, $"player_{player.Id}.json");
            var json = JsonSerializer.Serialize(player, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
            Console.WriteLine($"[DataService] Saved player {player.Id} to {filePath}");
        }

        private void LoadPlayers()
        {
            var files = Directory.GetFiles(_dataPath, "player_*.json");

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var player = JsonSerializer.Deserialize<Player>(json);

                    if (player != null)
                    {
                        _players[player.Id] = player;
                        if (player.Id >= _nextId)
                        {
                            _nextId = player.Id + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DataService] Error loading {file}: {ex.Message}");
                }
            }

            Console.WriteLine($"[DataService] Loaded {_players.Count} players from {_dataPath}");
        }

        public void SavaAllPlayers()
        {
            foreach (var player in _players.Values)
            {
                SavePlayer(player);
            }
        }
    }
}