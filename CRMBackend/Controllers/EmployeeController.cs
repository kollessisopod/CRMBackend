using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMBackend.Entities;
using CRMBackend.Services;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Runtime.CompilerServices;
using CRMBackend.Dtos;
using Npgsql;

namespace CRMBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly EmployeeServices _employeeServices;
    private readonly PlayerServices _playerServices;
    private readonly FeedbackServices _feedbackServices;
    private readonly NotificationServices _notificationServices;
    private readonly GameServices _gameServices;
    private readonly CampaignServices _campaignServices;
    private readonly AppDbContext _context;

    public EmployeeController(ILogger<EmployeeController> logger,
        AppDbContext context,
        EmployeeServices employeeServices,
        FeedbackServices feedbackServices,
        PlayerServices playerServices,
        NotificationServices notificationServices,
        GameServices gameServices,
        CampaignServices campaignServices)
    {
        _logger = logger;
        _context = context;
        _employeeServices = employeeServices;
        _feedbackServices = feedbackServices;
        _playerServices = playerServices;
        _notificationServices = notificationServices;
        _gameServices = gameServices;
        _campaignServices = campaignServices;
    }


    [HttpPost("EmployeeLogin")]
    public async Task<IActionResult> EmployeeLogin([FromForm] int id, [FromForm] string password)
    {
        try
        {
            var employee = await Task.Run(() => _employeeServices.GetEmployeeById(id));
            if (employee == null || employee.Password != password)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(new { employee.Username, employee.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error logging in: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetEmployees")]
    public async Task<IActionResult> GetEmployees()
    {
        try
        {
            var employees = await Task.Run(() => _employeeServices.GetEmployees());
            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching employees: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetFeedbacks")]
    public async Task<IActionResult> GetFeedbacks()
    {
        try
        {
            var feedbacks = await Task.Run(() => _feedbackServices.GetFeedbacks());
            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching feedbacks: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
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

    [HttpGet("GetCampaigns")]
    public async Task<IActionResult> GetCampaigns()
    {
        try
        {
            var campaigns = await Task.Run(() => _campaignServices.GetCampaigns());
            return Ok(campaigns);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching campaigns: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("CreateGame")]
    public async Task<IActionResult> CreateGame([FromForm] string name, [FromForm] string genre)
    {
        Game game = new()
        {
            Name = name,
            Genre = genre
        };
        try
        {
            await Task.Run(() => _gameServices.CreateGame(game));
            return Ok("Game created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating game: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("SendNotification")]
    public async Task<IActionResult> SendNotification([FromForm] string content)
    {
        Notification notification = new()
        {
            Content = content,
            IsRead = false,
        };

        try
        {
            var playerList = await Task.Run(() => _playerServices.GetPlayers());

            foreach (var player in playerList)
            {
                notification.NotificationId = 1;
                notification.PlayerId = player.Id;
                await Task.Run(() => _notificationServices.CreateNotification(notification));
            }

            return Ok("Notifications sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending notification: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("CreateCampaign")]
    public async Task<IActionResult> CreateCampaign([FromForm] string campaignInfo, [FromForm] bool hasReward, [FromForm] string rewardInfo)
    {
        try
        {
            var campaign = new Campaign
            {
                HasReward = hasReward,
                Info = campaignInfo,
                RewardInfo = rewardInfo
            };
            await Task.Run(() => _campaignServices.CreateCampaign(campaign));
            return Ok("Campaign created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating campaign: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("DeleteCampaign")]
    public async Task<IActionResult> DeleteCampaign([FromForm] int campaignId)
    {
        try
        {
            var campaign = await Task.Run(() => _campaignServices.GetCampaignById(campaignId));
            if (campaign == null)
            {
                return NotFound("Campaign not found");
            }
            await Task.Run(() => _campaignServices.DeleteCampaign(campaignId));
            return Ok("Campaign deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting campaign: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("AnnounceCampaign")]
    public async Task<IActionResult> AnnounceCampaign([FromForm] int campaignId)
    {
        try
        {
            var campaign = await Task.Run(() => _campaignServices.GetCampaignById(campaignId));
            if (campaign == null)
            {
                return NotFound("Campaign not found");
            }

            StringBuilder campaignStringBuilder = new StringBuilder()
                .Append("New Event: ")
                .Append(campaign.Info);

            if (campaign.HasReward)
            {
                campaignStringBuilder.Append(" with reward: ")
                    .Append(campaign.RewardInfo);
            }

            String campaignString = campaignStringBuilder.ToString();
            await SendNotification(campaignString);

            return Ok("Campaign announced successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error announcing campaign: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetTopTenGamesByPositivity")]
    public async Task<IActionResult> GetTopTenGamesByPositivity()
    {
        try
        {
            var gamesList = new List<(Game Game, decimal PositivityScore)>();

            var query = "SELECT * FROM get_top_10_games_by_avg_score();";
            var games = await _context.Database
                                      .SqlQueryRaw<PositivityGameDto>(query)
                                      .AsNoTracking()
                                      .ToListAsync();

            foreach (var g in games)
            {
                Game game = _gameServices.GetGameById(g.GameId);
                gamesList.Add((game, g.AvgScore));
            }

            return Ok(gamesList);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching recommended games: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetTopTenGamesByPopularity")]
    public async Task<IActionResult> GetTopTenGamesByPopularity()
    {
        try
        {
            var gamesList = new List<(Game Game, long PopularityScore)>();

            var query = "SELECT * FROM get_top_10_games_by_popularity();";
            var games = await _context.Database
                                      .SqlQueryRaw<PopularityGameDto>(query)
                                      .AsNoTracking()
                                      .ToListAsync();

            foreach (var g in games)
            {
                Game game = _gameServices.GetGameById(g.GameId);
                gamesList.Add((game, g.Popularity));
            }

            return Ok(gamesList);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching recommended games: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

}


