using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UntStudio.Server.Controllers;

[Authorize]
public sealed class AdminsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Menu()
    {
        return View();
    }

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return View();
    }
}
