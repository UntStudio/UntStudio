using Microsoft.AspNetCore.Mvc;

namespace UntStudio.Server.Controllers;

public sealed class HomeController : ControllerBase
{
    public HomeController()
    {
    }



    public IActionResult Index()
    {
        return Ok();
    }
}
