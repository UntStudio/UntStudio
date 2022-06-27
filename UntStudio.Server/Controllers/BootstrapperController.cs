using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UntStudio.Server.Data;
using UntStudio.Server.Models;
using UntStudio.Server.Strings;
using static UntStudio.Server.Models.RequestResponse;

namespace UntStudio.Server.Controllers;

public sealed class BootstrapperController : ControllerBase
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
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues value) == false)
        {
            return BadRequest();
        }

        if (value != "UntStudio.Loader")
        {
            return BadRequest();
        }

        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnowPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyValidator);

        if (keyValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.KeyValidationFailed)));
        }

        if (this.database.Data.ToList().Any(p => p.NotExpired == false && p.Key.Equals(key)) == false)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SpecifiedKeyNotFound)));
        }

        string file = Path.Combine(this.configuration["PluginsLoader:Path"]);
        return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(file)));
    }
}
