using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UntStudio.Server.Controllers;

public sealed class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> logger;



    public HomeController(ILogger<HomeController> logger)
    {
        this.logger = logger;
    }



    public IActionResult Index()
    {
        this.logger.LogWarning($"Maybe someone trying to crack us. IP: {ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4()}");
        return Ok();
    }
}
