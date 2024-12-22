using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CRMBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly AppDbContext _context;

    public EmployeeController(ILogger<EmployeeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("Template")]
    public async Task<IActionResult> Template()
    {
        try
        {
            var employees = await _context.Players.ToListAsync();
            return Ok();
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
            var employees = _context.Employees
                .FromSqlRaw("EXEC GetAllEmployees")
                .ToListAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching employees: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}



