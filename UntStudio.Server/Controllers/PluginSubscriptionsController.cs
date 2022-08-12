using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UntStudio.Server.Data;
using UntStudio.Server.Encryptors;
using UntStudio.Server.Knowns;
using UntStudio.Server.Models;
using UntStudio.Server.Strings;
using static UntStudio.Server.Models.RequestResponse;

namespace UntStudio.Server.Controllers;

public sealed class PluginSubscriptionsController : ControllerBase
{
    private readonly PluginSubscriptionsDatabaseContext database;

    private readonly IConfiguration configuration;

    private readonly IEncryptor encryptor;



    public PluginSubscriptionsController(PluginSubscriptionsDatabaseContext database, IConfiguration configuration, IEncryptor encryptor)
    {
        this.database = database;
        this.configuration = configuration;
        this.encryptor = encryptor;
    }



    public async Task<IActionResult> Load(string name)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.LicenseKey, out StringValues licenseKeyStringValue) == false)
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

        string licenseKey = licenseKeyStringValue.ToString();
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.LicenseKeyValidationFailed)));
        }

        name.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameValidator);

        if (nameValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.NameValidationFailed)));
        }

        string pluginFile = null;
        byte[] defaultBytes = null;
        byte[] encryptedBytes = null;
        PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.LicenseKey.Equals(licenseKey)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()))
            && p.Name.Equals(name)
            && p.Free);
        if (freePlugin != null)
        {
            if (freePlugin.Banned)
            {
                return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBanned)));
            }

            if (freePlugin.Expired)
            {
                return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionExpired)));
            }

            if (freePlugin.BlockedByOwner)
            {
                return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBlockedByOwner)));
            }

             pluginFile = Path.Combine(this.configuration["PluginsDirectory:Path"], string.Concat(name, ".dll"));
             defaultBytes = System.IO.File.ReadAllBytes(pluginFile);
             await this.encryptor.EncryptContentAsync(Convert.ToBase64String(defaultBytes), licenseKey);
            return Ok(Convert.ToBase64String(encryptedBytes));
        }

        PluginSubscription plugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.LicenseKey.Equals(licenseKey)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()))
            && p.Name.Equals(name));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SpecifiedLicenseKeyOrIPNotBindedOrNameNotFound)));
        }

        if (plugin.Banned)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBanned)));
        }

        if (plugin.Expired)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionExpired)));
        }

        if (plugin.BlockedByOwner)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBlockedByOwner)));
        }

        pluginFile = Path.Combine(this.configuration["PluginsDirectory:Path"], string.Concat(name, ".dll"));
        defaultBytes = System.IO.File.ReadAllBytes(pluginFile);
        encryptedBytes = await this.encryptor.EncryptContentAsync(Convert.ToBase64String(defaultBytes), licenseKey);
        return Ok(Convert.ToBase64String(encryptedBytes));
    }

    /*public IActionResult Block(string name)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.LicenseKey, out StringValues licenseKeyStringValue) == false)
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

        string licenseKey = licenseKeyStringValue.ToString();
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.LicenseKeyValidationFailed)));
        }

        name.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameValidator);

        if (nameValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.NameValidationFailed)));
        }

        *//*PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress))
            && p.Name.Equals(name)
            && p.Free);*//*
        PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.LicenseKey.Equals(licenseKey)
            && p.Name.Equals(name)
            && p.Free);
        if (freePlugin != null)
        {
            if (freePlugin.Banned)
            {
                return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBanned)));
            }

            if (freePlugin.Expired)
            {
                return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionExpired)));
            }

            if (freePlugin.BlockedByOwner)
            {
                return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionAlreadyBlocked)));
            }

            freePlugin.SetBlockedByOwner();
            this.database.Data.Update(freePlugin);
            this.database.SaveChanges();
            return Ok();
        }

        *//*PluginSubscription paidPlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress))
            && p.Name.Equals(name));*//*
        PluginSubscription paidPlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.LicenseKey.Equals(licenseKey)
            && p.Name.Equals(name));
        if (paidPlugin == null)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SpecifiedLicenseKeyOrIPNotBindedOrNameNotFound)));
        }

        if (paidPlugin.Banned)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBanned)));
        }

        if (paidPlugin.Expired)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionExpired)));
        }

        if (paidPlugin.BlockedByOwner)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionAlreadyBlocked)));
        }

        paidPlugin.SetBlockedByOwner();
        this.database.Data.Update(paidPlugin);
        this.database.SaveChanges();
        return Ok();
    }

    public IActionResult Unblock(string name)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.LicenseKey, out StringValues keyStringValue) == false)
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

        string licenseKey = keyStringValue.ToString();
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.LicenseKeyValidationFailed)));
        }

        name.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameValidator);

        if (nameValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.NameValidationFailed)));
        }

        *//*PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress))
            && p.Name.Equals(name)
            && p.Free);*//*
        PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.LicenseKey.Equals(licenseKey)
            && p.Name.Equals(name)
            && p.Free);
        if (freePlugin != null)
        {
            if (freePlugin.Banned)
            {
                return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBanned)));
            }

            if (freePlugin.Expired)
            {
                return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionExpired)));
            }

            if (freePlugin.UnblockedByOwner)
            {
                return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionAlreadyUnblocked)));
            }

            freePlugin.SetUnblockedByOwner();
            this.database.Data.Update(freePlugin);
            this.database.SaveChanges();
            return Ok();
        }

        *//*PluginSubscription paidPlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress))
            && p.Name.Equals(name));*//*
        PluginSubscription paidPlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.LicenseKey.Equals(licenseKey)
            && p.Name.Equals(name));
        if (paidPlugin == null)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SpecifiedLicenseKeyOrIPNotBindedOrNameNotFound)));
        }

        if (paidPlugin.Banned)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionBanned)));
        }

        if (paidPlugin.Expired)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionExpired)));
        }

        if (paidPlugin.UnblockedByOwner)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SubscriptionAlreadyUnblocked)));
        }

        paidPlugin.SetUnblockedByOwner();
        this.database.Data.Update(paidPlugin);
        this.database.SaveChanges();
        return Ok();
    }*/
}
