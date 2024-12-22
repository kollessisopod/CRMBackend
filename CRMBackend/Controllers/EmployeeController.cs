using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var employees = await _context.Employees.ToListAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching employees: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}