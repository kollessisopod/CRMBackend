using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRMBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class PostgresController : ControllerBase
{
    private readonly ILogger<PostgresController> _logger;
    private readonly AppDbContext _context;

    public PostgresController(ILogger<PostgresController> logger, AppDbContext context)
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
            _logger.LogError($"Error fetching products: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}