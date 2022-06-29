using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Linq;
using UntStudio.Server.Data;
using UntStudio.Server.Knowns;
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



    public IActionResult UnloadLoader()
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            Console.WriteLine("Errore #1!");
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            Console.WriteLine("Errore #2!");
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentLoaderValue)
        {
            Console.WriteLine("Errore #3!");
            return BadRequest();
        }

        string key = keyStringValue.ToString();
        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            Console.WriteLine("Errore #4!");
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.KeyValidationFailed)));
        }

        if (this.database.Data.ToList().Any(p => p.NotBannedOrExpired && p.Key.Equals(p.Key) && p.Free) ||
            this.database.Data.ToList().Any(p => p.NotBannedOrExpired && p.Key.Equals(p.Key)))
        {
            Console.WriteLine("Returned loader!");
            return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(this.configuration["PluginsLoader:Path"])));
        }

        Console.WriteLine("Errore #5!");
        return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBannedOrExpiredOrSpecifiedKeyNotFound)));
    }

    public IActionResult GetLoaderEntryPoint()
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentLoaderValue)
        {
            return BadRequest();
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

        if (this.database.Data.Any(p => p.Free && p.Key.Equals(key)) == false)
        {
            if (this.database.Data.ToList().Any(p => p.NotBannedOrExpired && p.Key.Equals(key)) == false)
            {
                return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBannedOrExpiredOrSpecifiedKeyNotFound)));
            }
        }

        return Ok(JsonConvert.SerializeObject(new LoaderEntryPoint
        (
            this.configuration["LoaderEntryPoint:Namespace"],
            this.configuration["LoaderEntryPoint:Class"],
            this.configuration["LoaderEntryPoint:Method"]))
        );
    }
}
