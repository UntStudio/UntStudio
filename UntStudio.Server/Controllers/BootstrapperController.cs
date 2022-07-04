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
    private readonly PluginSubscriptionsDatabaseContext database;

    private readonly IConfiguration configuration;



    public BootstrapperController(PluginSubscriptionsDatabaseContext database, IConfiguration configuration)
    {
        this.database = database;
        this.configuration = configuration;
    }



    public IActionResult UnloadLoader()
    {
        Console.WriteLine("----------------------------------LOCAL IP ADDRESS: " + HttpContext.Connection.LocalIpAddress.ToString());
        Console.WriteLine("----------------------------------REMOTE IP ADDRESS: " + HttpContext.Connection.RemoteIpAddress.ToString());
        Console.WriteLine("----------------------------------REMOTE IP MapToIPv4 ADDRESS: " + HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            Console.WriteLine("unload loader------------------------Errore #1!");
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            Console.WriteLine("unload loader------------------------Errore #2!");
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentLoaderValue)
        {
            Console.WriteLine("unload loader------------------------Errore #3!");
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

        /*if (this.database.Data.ToList().Any(p => 
            p.NotBannedOrExpired
            && p.Key.Equals(p.Key)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString())) 
            && p.Free))
        {
            return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(this.configuration["PluginsLoader:Path"])));
        }*/

        if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.Key.Equals(p.Key)
            && p.Free))
        {
            return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(this.configuration["PluginsLoader:Path"])));
        }

        /*if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.Key.Equals(p.Key)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()))))
        {
            return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(this.configuration["PluginsLoader:Path"])));
        }*/

        if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.Key.Equals(p.Key)))
        {
            return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(this.configuration["PluginsLoader:Path"])));
        }

        return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedKeyNotFound)));
    }

    public IActionResult GetLoaderEntryPoint()
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            Console.WriteLine("loader entry point------------------------Errore #1!");
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            Console.WriteLine("loader entry point------------------------Errore #2!");
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentLoaderValue)
        {
            Console.WriteLine("loader entry point------------------------Errore #3!");
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

        /*if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress))
            && p.Key.Equals(p.Key)
            && p.Free) == false)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedKeyNotFound)));
        }*/

        if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.Key.Equals(p.Key)
            && p.Free))
        {
            return Ok(JsonConvert.SerializeObject(new LoaderEntryPoint
            (
                this.configuration["LoaderEntryPoint:Namespace"],
                this.configuration["LoaderEntryPoint:Class"],
                this.configuration["LoaderEntryPoint:Method"]))
            );
        }

        /*if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress))
            && p.Key.Equals(p.Key)) == false)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedKeyNotFound)));
        }*/

        if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.Key.Equals(p.Key)))
        {
            return Ok(JsonConvert.SerializeObject(new LoaderEntryPoint
            (
                this.configuration["LoaderEntryPoint:Namespace"],
                this.configuration["LoaderEntryPoint:Class"],
                this.configuration["LoaderEntryPoint:Method"]))
            );
        }

        return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedKeyNotFound)));
    }
}
