using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace GraphService.Controllers;

[ApiController]
[Route("[controller]")]
public class GraphApiController : ControllerBase
{
    private ILogger<GraphApiController> _logger;
    private ModStatsContext _context;
    
    public GraphApiController(ILogger<GraphApiController> logger, ModStatsContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("GetDownloadsGraph")]
    public string Get()
    {
        return null;
    }
}