using Microsoft.AspNetCore.Mvc;

namespace ParkingLot.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ParkingSpotsController : ControllerBase
{
    
    private readonly ILogger<ParkingSpotsController> _logger;

    public ParkingSpotsController(ILogger<ParkingSpotsController> logger)
    {
        _logger = logger;
    }
}