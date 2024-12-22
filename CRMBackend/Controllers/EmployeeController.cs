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
    private readonly AppDbContext _context;

    public EmployeeController(ILogger<EmployeeController> logger, 
        AppDbContext context,
        EmployeeServices employeeServices)
    {
        _logger = logger;
        _context = context;
        _employeeServices = employeeServices;
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



