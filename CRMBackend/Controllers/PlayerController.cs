using Microsoft.AspNetCore.Mvc;
using CRMBackend.Requests;
using CRMBackend.Services;
using Microsoft.EntityFrameworkCore;
using CRMBackend.Entities;

namespace CRMBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly PlayerServices _playerServices;
    private readonly FeedbackServices _feedbackServices;
    private readonly AppDbContext _context;

    public PlayerController(ILogger<PlayerController> logger,
        AppDbContext context,
        PlayerServices playerServices,
        FeedbackServices feedbackServices)
    {
        _logger = logger;
        _context = context;
        _playerServices = playerServices;
        _feedbackServices = feedbackServices;
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


    [HttpPost("SubmitFeedback")]
    public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackRequest request)
    {
        try
        {
            var player = _playerServices.GetPlayerByUsername(request.Username);
            if (player == null)
            {
                return NotFound("Player not found");
            }

            Feedback feedback = new()
            {
                Id = 1,
                PlayerId = player.Id,
                PlayerUsername = request.Username,
                FeedbackContent = request.FeedbackContent,
                FeedbackType = request.FeedbackType
            };

            if (_feedbackServices.CreateFeedback(feedback) != null)
            {
                return Ok("Feedback inserted successfully");
            } 
            else
            {
                return StatusCode(500, "Internal server error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error inserting feedback: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
