using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UntStudio.Server.Data;
using UntStudio.Server.Models;
using UntStudio.Server.Repositories;
using UntStudio.Server.Strings;
using static UntStudio.Server.Models.RequestResponse;

namespace UntStudio.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PluginsController : Controller
{
    private readonly PluginsDatabaseContext database;

    private readonly IConfiguration configuration;

    private readonly IHashesVerifierRepository loaderHashesVerifierRepository;



    public PluginsController(PluginsDatabaseContext database, IConfiguration configuration, IHashesVerifierRepository loaderHashesVerifierRepository)
    {
        this.database = database;
        this.configuration = configuration;
        this.loaderHashesVerifierRepository = loaderHashesVerifierRepository;
    }



    private const int KeyLength = 19;

    [HttpGet]
    public IActionResult GetPlugin(byte[] loaderBytes, string key)
    {
        if (this.loaderHashesVerifierRepository.Verify(loaderBytes) == false)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.VersionOutdated)));
        }

        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KeyLength)
            .Return(out IStringValidator keyValidator);

        if (keyValidator.Failed)
        {
            return BadRequest();
        }

        Plugin plugin = this.database.Data.FirstOrDefault(p => p.Key.Equals(key));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.NotFound)));
        }

        if (plugin.Expired)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionExpired)));
        }

        return Ok(JsonConvert.SerializeObject(plugin));
    }

    [HttpGet]
    public IActionResult Unload(byte[] loaderBytes, string key, string pluginName)
    {
        if (this.loaderHashesVerifierRepository.Verify(loaderBytes) == false)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.VersionOutdated)));
        }

        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KeyLength)
            .Return(out IStringValidator keyValidator);

        if (keyValidator.Failed)
        {
            return BadRequest();
        }

        pluginName.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator pluginNameValidator);

        if (pluginNameValidator.Failed)
        {
            return BadRequest();
        }

        Plugin plugin = this.database.Data.FirstOrDefault(p => p.Key.Equals(key) && p.Name.Equals(pluginName));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.NotFound)));
        }

        if (plugin.Expired)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionExpired)));
        }

        string file = Path.Combine(this.configuration["PluginsDirectory:Path"], string.Concat(pluginName, ".dll"));
        return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(file)));
    }
}
