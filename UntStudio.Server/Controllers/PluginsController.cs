using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UntStudio.Server.Data;
using UntStudio.Server.Models;
using UntStudio.Server.Strings;
using static UntStudio.Server.Models.PluginRequestResult;

namespace UntStudio.Server.Controllers;

public class PluginsController : Controller
{
    private readonly PluginsDatabaseContext database;

    private readonly IConfiguration configuration;



    public PluginsController(PluginsDatabaseContext database, IConfiguration configuration)
    {
        this.database = database;
        this.configuration = configuration;
    }



    public IActionResult GetPlugin(string key)
    {
        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(19)
            .Return(out IStringValidator keyValidator);

        if (keyValidator.Failed)
        {
            return BadRequest();
        }

        Plugin plugin = this.database.Data.FirstOrDefault(p => p.Key.Equals(key));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new PluginRequestResult(CodeResponse.NotFound)));
        }

        if (plugin.Expired)
        {
            return Content(JsonConvert.SerializeObject(new PluginRequestResult(CodeResponse.SubscriptionExpired)));
        }

        return Ok(JsonConvert.SerializeObject(plugin));
    }

    public IActionResult Unload(string key, string pluginName)
    {
        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(19)
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
            return Content(JsonConvert.SerializeObject(new PluginRequestResult(CodeResponse.NotFound)));
        }

        if (plugin.Expired)
        {
            return Content(JsonConvert.SerializeObject(new PluginRequestResult(CodeResponse.SubscriptionExpired)));
        }

        string file = Path.Combine(this.configuration["PluginsDirectory:Path"], pluginName + ".dll");
        return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(file)));
    }
}
