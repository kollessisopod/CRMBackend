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
    private readonly FeedbackServices _feedbackServices;
    private readonly AppDbContext _context;

    public EmployeeController(ILogger<EmployeeController> logger, 
        AppDbContext context,
        EmployeeServices employeeServices,
        FeedbackServices feedbackServices)
    {
        _logger = logger;
        _context = context;
        _employeeServices = employeeServices;
        _feedbackServices = feedbackServices;
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



    [HttpGet("ProcedureTemplate")]
    public async Task<IActionResult> ProcedureTemplate()
    {

        try
        {
            var employees = await _context.Players
                .FromSqlRaw("SELECT * FROM get_all_players()")
                .ToListAsync();

            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching employees: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpPost("ProcedureSelectTemplate")]
    public async Task<IActionResult> ProcedureSelectTemplate(UsernameRequest request)
    {
        try
        {
            var employees = await _context.Players
                .FromSqlRaw("SELECT * FROM get_all_players() WHERE p_name = {0}", request.username)
                .ToListAsync();

            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching employees: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}



