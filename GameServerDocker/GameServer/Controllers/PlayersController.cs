// API 엔드포인트 설정하는 스크립트

using GameServer.Models;
using GameServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly PlayerService _playerService;

        public PlayersController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost]
        public ActionResult<Player> CreatePlayer([FromBody] string name)
        {
            var player = _playerService.CreatePlayer(name);
            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id}, player);
        }

        [HttpGet("{id}")]
        public ActionResult<Player> GetPlayer(int id)
        {
            var player = _playerService.GetPlayer(id);
            if (player == null)
            {
                return NotFound();
            }
            return player;
        }

        [HttpGet]
        public ActionResult<List<Player>> GetAllPlayer()
        {
            return _playerService.GetAllPlayers();
        }

        [HttpPut("{id}/score")]
        public IActionResult UpdateScore(int id, [FromBody] int score)
        {
            if (!_playerService.UpdateScore(id, score))
                return NotFound();
            return NoContent();
        }

    }
}
