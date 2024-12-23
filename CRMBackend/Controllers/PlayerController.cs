using Microsoft.AspNetCore.Mvc;
using CRMBackend.Requests;
using CRMBackend.Services;
using Microsoft.EntityFrameworkCore;
using CRMBackend.Entities;
using CRMBackend.Auth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace CRMBackend.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly PlayerServices _playerServices;
    private readonly FeedbackServices _feedbackServices;
    private readonly PlayerGameServices _playerGameServices;
    private readonly TokenHelper _tokenHelper;

    private readonly AppDbContext _context;

    public PlayerController(ILogger<PlayerController> logger,
        AppDbContext context,
        PlayerServices playerServices,
        FeedbackServices feedbackServices,
        PlayerGameServices playerGameServices,
        TokenHelper tokenHelper)
    {
        _logger = logger;
        _context = context;
        _playerServices = playerServices;
        _feedbackServices = feedbackServices;
        _playerGameServices = playerGameServices;
        _tokenHelper = tokenHelper;
    }


    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        Player? user = await _context.Players.FirstOrDefaultAsync(x => x.Username == request.Username);
        if (user == null )
        {
            return Unauthorized("Invalid username");
        }

        if (user.Password != request.Password) 
        {
            return Unauthorized("Invalid password.");
        }


        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username)
        };

        var token = _tokenHelper.CreateToken(claims, TimeSpan.FromHours(1));
        return Ok(new { Token = token });
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

    [HttpGet("ListGames")]
    public async Task<IActionResult> ListGames()
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
            _playerGameServices.CreatePlayerGame(playerGame);
            _context.SaveChanges();
            return Ok("Game rated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error rating game: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("ChangeGameScore")]
    public async Task<IActionResult> ChangeGameScore(RateGameRequest request)
    {
        try
        {
            var player = await Task.Run(() => _playerServices.GetPlayerByUsername(request.Username));
            if (player == null)
            {
                return NotFound("Player not found");
            }
            PlayerGame? playerGame = _playerGameServices.GetPlayerGameByPlayerAndGameId(player.Id, request.GameId);
            if (playerGame == null)
            {
                return NotFound("Game not found");
            }
            playerGame.Score = request.Score;
            _playerGameServices.UpdatePlayerGame(playerGame);
            return Ok("Game score updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating game score: {ex.Message}");
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


}
