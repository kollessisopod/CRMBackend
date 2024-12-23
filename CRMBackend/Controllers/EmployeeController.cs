using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMBackend.Entities;
using CRMBackend.Requests;
using CRMBackend.Services;
using System.Data;
using System.Data.SqlClient;

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
    private readonly AppDbContext _context;

    public EmployeeController(ILogger<EmployeeController> logger,
        AppDbContext context,
        EmployeeServices employeeServices,
        FeedbackServices feedbackServices,
        PlayerServices playerServices,
        NotificationServices notificationServices,
        GameServices gameServices)
    {
        _logger = logger;
        _context = context;
        _employeeServices = employeeServices;
        _feedbackServices = feedbackServices;
        _playerServices = playerServices;
        _notificationServices = notificationServices;
        _gameServices = gameServices;
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

    [HttpPost("CreateGame")]
    public async Task<IActionResult> CreateGame(CreateGameRequest request)
    {
        Game game = new()
        {
            Id = request.Id,
            Name = request.Name,
            Genre = request.Genre
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
    public async Task<IActionResult> SendNotification(SendNotificationRequest request)
    {
        Notification notification = new()
        {
            Content = request.Content,
            IsRead = false,
        };

        try
        {
            var playerList = await Task.Run(() => _playerServices.GetPlayers());

            foreach (var player in playerList)
            {
                notification.NotificationId = player.Id + 1;
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
}


