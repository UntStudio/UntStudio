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



    public IActionResult LoadLoader()
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.LicenseKey, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentBootstrapperValue)
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
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.LicenseKeyValidationFailed)));
        }

        if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString())
            && p.LicenseKey.Equals(p.LicenseKey)
            && p.Free)))
        {
            return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(this.configuration["PluginsLoader:Path"])));
        }

        if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString())
            && p.LicenseKey.Equals(p.LicenseKey))))
        {
            return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(this.configuration["PluginsLoader:Path"])));
        }

        return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedLicenseKeyNotFound)));
    }

    public IActionResult GetLoaderEntryPoint()
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.LicenseKey, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentBootstrapperValue)
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
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.LicenseKeyValidationFailed)));
        }

        if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString())
            && p.LicenseKey.Equals(p.LicenseKey)
            && p.Free)))
        {
            return Ok(JsonConvert.SerializeObject(new LoaderEntryPoint
            (
                this.configuration["LoaderEntryPoint:Namespace"],
                this.configuration["LoaderEntryPoint:Class"],
                this.configuration["LoaderEntryPoint:Method"]))
            );
        }

        if (this.database.Data.ToList().Any(p =>
            p.NotBannedOrExpired
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString())
            && p.LicenseKey.Equals(p.LicenseKey))))
        {
            return Ok(JsonConvert.SerializeObject(new LoaderEntryPoint
            (
                this.configuration["LoaderEntryPoint:Namespace"],
                this.configuration["LoaderEntryPoint:Class"],
                this.configuration["LoaderEntryPoint:Method"]))
            );
        }

        return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedLicenseKeyNotFound)));
    }
}
