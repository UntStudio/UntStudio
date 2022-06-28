using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UntStudio.Server.Data;
using UntStudio.Server.Knowns;
using UntStudio.Server.Models;
using UntStudio.Server.Repositories;
using UntStudio.Server.Strings;
using static UntStudio.Server.Models.RequestResponse;

namespace UntStudio.Server.Controllers;

public sealed class PluginSubscriptionsController : ControllerBase
{
    private readonly PluginsDatabaseContext database;

    private readonly IConfiguration configuration;

    private readonly IHashesVerifierRepository loaderHashesVerifierRepository;



    public PluginSubscriptionsController(PluginsDatabaseContext database, IConfiguration configuration, IHashesVerifierRepository loaderHashesVerifierRepository)
    {
        this.database = database;
        this.configuration = configuration;
        this.loaderHashesVerifierRepository = loaderHashesVerifierRepository;
    }



    public IActionResult Unload(byte[] loaderBytes, string name)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues agentStringValue) == false)
        {
            return BadRequest();
        }

        if (agentStringValue != KnownHeaders.UserAgentLoaderValue)
        {
            return BadRequest();
        }

        if (this.loaderHashesVerifierRepository.Verify(loaderBytes) == false)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.VersionOutdated)));
        }

        string key = keyStringValue.ToString();
        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.KeyValidationFailed)));
        }

        name.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameValidator);

        if (nameValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.NameValidationFailed)));
        }

        PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p => p.Key.Equals(key) && p.Name.Equals(name) && p.Free);
        if (freePlugin != null)
        {
            string freePluginFile = Path.Combine(this.configuration["PluginsDirectory:Path"], string.Concat(name, ".dll"));
            return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(freePluginFile)));
        }

        PluginSubscription plugin = this.database.Data.ToList().FirstOrDefault(p => 
            p.NotExpired
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress)) 
            && p.Key.Equals(key) 
            && p.Name.Equals(name));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.IPNotBindedOrSpecifiedKeyOrNameNotFound)));
        }

        if (plugin.Banned)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBanned)));
        }

        if (plugin.Expired)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionExpired)));
        }

        string pluginFile = Path.Combine(this.configuration["PluginsDirectory:Path"], string.Concat(name, ".dll"));
        return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(pluginFile)));
    }
}
