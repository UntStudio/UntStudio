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



    public IActionResult Unload(/*byte[] loaderBytes, */string name)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            Console.WriteLine("UnloadPlugin------------------------Errore #1!");
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues agentStringValue) == false)
        {
            Console.WriteLine("UnloadPlugin------------------------Errore #2!");
            return BadRequest();
        }

        if (agentStringValue != KnownHeaders.UserAgentLoaderValue)
        {
            Console.WriteLine("UnloadPlugin------------------------Errore #3!");
            return BadRequest();
        }

        /*if (this.loaderHashesVerifierRepository.Verify(loaderBytes) == false)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.VersionOutdated)));
        }*/

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

        /*PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p => 
            p.Key.Equals(key) 
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()))
            && p.Name.Equals(name) 
            && p.Free);*/
        PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
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

            string freePluginFile = Path.Combine(this.configuration["PluginsDirectory:Path"], string.Concat(name, ".dll"));
            return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(freePluginFile)));
        }

        /*PluginSubscription plugin = this.database.Data.ToList().FirstOrDefault(p => 
            p.Key.Equals(key) 
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress)) 
            && p.Name.Equals(name));*/
        PluginSubscription plugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.Name.Equals(name));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SpecifiedKeyOrIPNotBindedOrNameNotFound)));
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

        string pluginFile = Path.Combine(this.configuration["PluginsDirectory:Path"], string.Concat(name, ".dll"));
        return Ok(Convert.ToBase64String(System.IO.File.ReadAllBytes(pluginFile)));
    }

    public IActionResult Block(string name)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            Console.WriteLine("Block------------------------Errore #1!");
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues agentStringValue) == false)
        {
            Console.WriteLine("Block------------------------Errore #2!");
            return BadRequest();
        }

        if (agentStringValue != KnownHeaders.UserAgentLoaderValue)
        {
            Console.WriteLine("Block------------------------Errore #3!");
            return BadRequest();
        }

        /*if (this.loaderHashesVerifierRepository.Verify(loaderBytes) == false)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.VersionOutdated)));
        }*/

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

        /*PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress))
            && p.Name.Equals(name)
            && p.Free);*/
        PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
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

        /*PluginSubscription paidPlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress))
            && p.Name.Equals(name));*/
        PluginSubscription paidPlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.Name.Equals(name));
        if (paidPlugin == null)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SpecifiedKeyOrIPNotBindedOrNameNotFound)));
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
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            Console.WriteLine("Unblock------------------------Errore #1!");
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues agentStringValue) == false)
        {
            Console.WriteLine("Unblock------------------------Errore #2!");
            return BadRequest();
        }

        if (agentStringValue != KnownHeaders.UserAgentLoaderValue)
        {
            Console.WriteLine("Unblock------------------------Errore #3!");
            return BadRequest();
        }

        /*if (this.loaderHashesVerifierRepository.Verify(loaderBytes) == false)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.VersionOutdated)));
        }*/

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

        /*PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress))
            && p.Name.Equals(name)
            && p.Free);*/
        PluginSubscription freePlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
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

        /*PluginSubscription paidPlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.AllowedAddressesParsed.Any(a => a.Equals(ControllerContext.HttpContext.Connection.RemoteIpAddress))
            && p.Name.Equals(name));*/
        PluginSubscription paidPlugin = this.database.Data.ToList().FirstOrDefault(p =>
            p.Key.Equals(key)
            && p.Name.Equals(name));
        if (paidPlugin == null)
        {
            return Content(JsonConvert.SerializeObject(new RequestResponse(CodeResponse.SpecifiedKeyOrIPNotBindedOrNameNotFound)));
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
    }
}
