using Microsoft.AspNetCore.Mvc;
using CRMBackend.Requests;
using CRMBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace CRMBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly PlayerServices _playerServices;
    private readonly AppDbContext _context;

    public PlayerController(ILogger<PlayerController> logger,
        AppDbContext context,
        PlayerServices playerServices)
    {
        _logger = logger;
        _context = context;
        _playerServices = playerServices;
    }

    [HttpGet("GetPlayers")]
    public async Task<IActionResult> GetPlayers()
    {
        try
        {
            var players = await Task.Run(() => _playerServices.GetPlayers());
            return Ok(players);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching players: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("InsertFeedback")]
    public async Task<IActionResult> InsertFeedback([FromBody] InsertFeedbackRequest request)
    {
        try
        {
            var player = _playerServices.GetPlayerById(request.Username);
            if (player == null)
            {
                return NotFound("Player not found");
            }
            player.FeedbackContent = request.FeedbackContent;
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
            return Ok("Feedback inserted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error inserting feedback: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
