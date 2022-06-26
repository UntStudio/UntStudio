using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UntStudio.Server.Data;
using UntStudio.Server.Models;
using UntStudio.Server.Strings;
using static UntStudio.Server.Models.RequestResult;

namespace UntStudio.Server.Controllers;

public sealed class BootstrapperController : Controller
{
    private readonly PluginsDatabaseContext database;

    private readonly IConfiguration configuration;



    public BootstrapperController(PluginsDatabaseContext database, IConfiguration configuration)
    {
        this.database = database;
        this.configuration = configuration;
    }



    public IActionResult UnloadLoader(string key)
    {
        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(19)
            .Return(out IStringValidator keyValidator);

        if (keyValidator.Failed)
        {
            return BadRequest();
        }

        if (this.database.Data.Any(p => p.Key.Equals(key) && p.NotExpired) == false)
        {
            return Content(JsonConvert.SerializeObject(new RequestResult(CodeResponse.NotFoundOrSubscriptionExpired)));
        }

        string file = Path.Combine(this.configuration["PluginsLoader:Path"]);
        return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(file)));
    }
}
