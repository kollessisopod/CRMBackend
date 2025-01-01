using Microsoft.AspNetCore.Mvc;
using CRMBackend.Requests;
using CRMBackend.Services;
using Microsoft.EntityFrameworkCore;
using CRMBackend.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CRMBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly PlayerServices _playerServices;
    private readonly FeedbackServices _feedbackServices;
    private readonly PlayerGameServices _playerGameServices;
    private readonly NotificationServices _notificationServices;
    private readonly GameServices _gameServices;
    private readonly CampaignServices _campaignServices;  


    private readonly AppDbContext _context;

    public PlayerController(ILogger<PlayerController> logger,
        AppDbContext context,
        PlayerServices playerServices,
        FeedbackServices feedbackServices,
        PlayerGameServices playerGameServices,
        NotificationServices notificationServices,
        GameServices gameServices,
        CampaignServices campaignServices)
    {
        _logger = logger;
        _context = context;
        _playerServices = playerServices;
        _feedbackServices = feedbackServices;
        _playerGameServices = playerGameServices;
        _notificationServices = notificationServices;
        _gameServices = gameServices;
        _campaignServices = campaignServices;
    }




    [HttpPost("PlayerLogin")]
    public async Task<IActionResult> PlayerLogin(PlayerLoginRequest request)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerByUsername(request.Username));
            if (player == null || player.Password != request.Password)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(new { player.Username, player.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error logging in: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("SubmitFeedback")]
    public async Task<IActionResult> SubmitFeedback(SubmitFeedbackRequest request)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerByUsername(request.Username));
            if (player == null)
            {
                return NotFound("Player not found");
            }

            Feedback feedback = new()
            {
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

    [HttpGet("GetGames")]
    public async Task<IActionResult> GetGames()
    {
        try
        {
            var games = await Task.Run(() => _context.Games.ToList());
            return Ok(games);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching games: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("ListPlayedGames")]
    public async Task<IActionResult> ListPlayedGames(UsernameRequest request)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerByUsername(request.Username));
            if (player == null)
            {
                return NotFound("Player not found");
            }

            var playedGames = await Task.Run(() => _playerGameServices.GetPlayerGamesByPlayerId(player.Id));
            if (playedGames == null)
            {
                return NotFound("No games found");
            }
            var gamesIds = playedGames.Select(pg => pg.GameId).ToList();
            var games = await Task.Run(() => _context.Games.Where(g => gamesIds.Contains(g.Id)).ToList());
            return Ok(playedGames);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching played games: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("SubmitGameScore")]
    public async Task<IActionResult> RateGame(RateGameRequest request)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerByUsername(request.Username));
            if (player == null)
            {
                return NotFound("Player not found");
            }
            
            PlayerGame playerGame = new PlayerGame
            {
                PlayerId = player.Id,
                GameId = request.GameId,
                Score = request.Score
            };
            
            int status = _playerGameServices.CreatePlayerGame(playerGame);
            _context.SaveChanges();
            if (status == 0)
            {
                return Ok("Game rated successfully");
            }
            else
            {
                return Ok("Game rate updated");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error rating game: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("DeleteGameScore")]
    public async Task<IActionResult> DeleteGameScore(RateGameRequest request)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerByUsername(request.Username));
            if (player == null)
            {
                return NotFound("Player not found");
            }
            _playerGameServices.DeletePlayerGame(player.Id, request.GameId);
            return Ok("Game score deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting game score: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("ListNotifications")]
    public async Task<IActionResult> ListNotifications(UsernameRequest request)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerByUsername(request.Username));
            if (player == null)
            {
                return NotFound("Player not found");
            }
            var notifications = await Task.Run(() => _context.Notifications.Where(n => n.PlayerId == player.Id).ToList());
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching notifications: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("MarkNotificationRead")]
    public async Task<IActionResult> MarkNotificationRead(MarkNotificationReadRequest request)
    {
        try
        {
            var notification = await Task.Run(() => _notificationServices.GetNotificationById(request.NotificationId));
            if (notification == null)
            {
                return NotFound("Notification not found");
            }
            await Task.Run(() => _notificationServices.MarkNotificationRead(notification));
            return Ok("Notification marked as read");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error marking notification as read: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("DeleteNotification")]
    public async Task<IActionResult> DeleteNotification(DeleteNotificationRequest request)
    {
        try
        {
            var notification = await Task.Run(() => _notificationServices.GetNotificationById(request.NotificationId));
            if (notification == null)
            {
                return NotFound("Notification not found");
            }
            await Task.Run(() => _notificationServices.DeleteNotification(request.PlayerId, request.NotificationId));
            return Ok("Notification deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting notification: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }


}
