using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace UntStudio.Server.Controllers;

public sealed class HomeController : ControllerBase
{
    /*private readonly ILogger<HomeController> logger;*/
    private readonly IConfiguration configuration;

    public HomeController(/*ILogger<HomeController> logger, */IConfiguration configuration)
    {
        /*this.logger = logger;*/
        this.configuration = configuration;
    }



    public IActionResult Index()
    {
        //this.logger.LogWarning($"Maybe someone trying to crack us. IP: {ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4()}");
        return Ok();
    }

    public IActionResult GetFile()
    {
        string filePath = Path.Combine(this.configuration["PluginsLoader:Path"]);
        StringBuilder stringBuilder = new StringBuilder()
            .Append("Path #1: " + AppContext.BaseDirectory)
            .Append("\n" + (System.IO.File.Exists(filePath) ? "File Exists" : "Not found"));

        return Content(stringBuilder.ToString());
    }
}
