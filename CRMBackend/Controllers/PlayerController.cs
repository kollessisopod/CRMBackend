using Microsoft.AspNetCore.Mvc;
using CRMBackend.Services;
using Microsoft.EntityFrameworkCore;
using CRMBackend.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;
using CRMBackend.Dtos;

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
    public async Task<IActionResult> PlayerLogin([FromForm] string username, [FromForm] string password)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerByUsername(username));
            if (player == null || player.Password != password)
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
    public async Task<IActionResult> SubmitFeedback([FromForm] int id, [FromForm] string feedbackType, [FromForm] string feedbackContent)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerById(id));
            if (player == null)
            {
                return NotFound("Player not found");
            }

            Feedback feedback = new()
            {
                PlayerId = player.Id,
                PlayerUsername = player.Username,
                FeedbackContent = feedbackContent,
                FeedbackType = feedbackType
            };

            if (_feedbackServices.CreateFeedback(feedback) != null)
            {
                return Ok(new { success = true, message = "Feedback inserted successfully" });
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

    [HttpGet("GetPlayerGameByPlayerAndGameId")]
    public async Task<IActionResult> GetPlayerGameByPlayerAndGameId([FromForm] int playerId, [FromForm] int gameId)
    {
        try
        {
            
            var playerGame = await Task.Run(() => _playerGameServices.GetPlayerGameByPlayerAndGameId(playerId, gameId));

            return Ok(playerGame);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching game: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("CreatePlayer")]
    public async Task<IActionResult> CreatePlayer([FromForm] string username, [FromForm] string password, [FromForm] string email, [FromForm] string phoneNumber)
    {
        try
        {
            var player = new Player
            {
                Username = username,
                Password = password,
                Email = email,
                PhoneNumber = phoneNumber,
                LastOnline = DateTime.Now

            };
            var existingPlayer = await Task.Run(() => _playerServices.GetPlayerByUsername(username));
            if (existingPlayer != null)
            {
                return BadRequest("Username already exists");
            }
            _playerServices.CreatePlayer(player);
            _context.SaveChanges();
            return Ok("Player created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating player: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("ListPlayedGames")]
    public async Task<IActionResult> ListPlayedGames([FromForm] int id)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerById(id));
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

            var games = await Task.WhenAll(
                gamesIds.Select(async gameId =>
                {
                    var game = _gameServices.GetGameById(gameId);
                    var playedGame = playedGames.FirstOrDefault(pg => pg.GameId == gameId);
                    return new
                    {
                        Game = game,
                        Score = playedGame?.Score, // Use null conditional in case playedGame is null
                    };
                })
            );

            return Ok(games);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching played games: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("SubmitGameScore")]
    public async Task<IActionResult> SubmitGameScore([FromForm] int id, [FromForm] int gameId, [FromForm] int score)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerById(id));
            if (player == null)
            {
                return NotFound("Player not found");
            }

            PlayerGame playerGame = new PlayerGame
            {
                PlayerId = player.Id,
                GameId = gameId,
                Score = score
            };

            int status = _playerGameServices.CreatePlayerGame(playerGame);
            _context.SaveChanges();
            if (status == 0)
            {
                return Ok(new { success = true, message = "Created" });
            }
            else
            {
                return Ok(new { success = true, message = "Updated" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error rating game: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("DeleteGameScore")]
    public async Task<IActionResult> DeleteGameScore([FromForm] int id, [FromForm] int gameId)
    {
        try
        {
            var player = await Task.Run(() => _playerGameServices.GetPlayerGameByPlayerAndGameId(id, gameId));
            if (player == null)
            {
                return NotFound("Player not found");
            }
            _playerGameServices.DeletePlayerGame(id, gameId);
            return Ok("Game score deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting game score: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("ListNotifications")]
    public async Task<IActionResult> ListNotifications([FromForm] int id)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerById(id));
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
    public async Task<IActionResult> MarkNotificationRead([FromForm] int notificationId)
    {
        try
        {
            var notification = await Task.Run(() => _notificationServices.GetNotificationById(notificationId));
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
    public async Task<IActionResult> DeleteNotification([FromForm] int notificationId)
    {
        try
        {
            var notification = await Task.Run(() => _notificationServices.GetNotificationById(notificationId));
            if (notification == null)
            {
                return NotFound("Notification not found");
            }
            await Task.Run(() => _notificationServices.DeleteNotification(notificationId));
            return Ok("Notification deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting notification: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("ListRecommendedGames")]
    public async Task<IActionResult> ListRecommendedGames([FromForm] int id)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerById(id));
            if (player == null)
            {
                return NotFound("Player not found");
            }

            var recommendedGames = new List<Game>();

            // Use raw SQL query to call the stored function directly
            var query = "SELECT * FROM get_recommended_games_for_player(@playerId);";
            var games = await _context.Database
                                      .SqlQueryRaw<RecommendedGameDto>(query, new NpgsqlParameter("@playerId", id))
                                      .AsNoTracking()
                                      .ToListAsync();

            Console.WriteLine("GAMES>" + games);

            foreach (var g in games)
            {
                Game game = _gameServices.GetGameById(g.RecommendedGameId);
                recommendedGames.Add(game);
            }

            return Ok(recommendedGames);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching recommended games: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }




}