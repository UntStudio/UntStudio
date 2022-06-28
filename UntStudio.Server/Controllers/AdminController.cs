using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UntStudio.Server.Data;
using UntStudio.Server.Knowns;
using UntStudio.Server.Models;
using UntStudio.Server.Strings;
using static UntStudio.Server.Models.AdminRequestResponse;

namespace UntStudio.Server.Controllers;

public sealed class AdminController : ControllerBase
{
    private readonly PluginsDatabaseContext database;



    public AdminController(PluginsDatabaseContext database)
    {
        this.database = database;
    }



    public IActionResult AddSubscription(string name, string allowedAddresses, int days)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
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
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        allowedAddresses.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator allowedAddressesValidator);

        if (allowedAddressesValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.AllowedAddressesNotSpecified)));
        }

        PluginSubscription plugin = new PluginSubscription(name, key, allowedAddresses, DateTime.Now.AddDays(days));
        this.database.Data.Add(plugin);
        this.database.SaveChangesAsync();

        return Ok(JsonConvert.SerializeObject(plugin));
    }

    public IActionResult AddFreeSubscription(string name)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
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
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        name.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameStringValidator);

        if (nameStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NameValidationFailed)));
        }

        PluginSubscription plugin = new PluginSubscription(name, key);
        plugin.SetFree();
        this.database.Data.Add(plugin);
        this.database.SaveChanges();

        return Ok(JsonConvert.SerializeObject(plugin));
    }

    public IActionResult BanSubscription(string name)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
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
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        name.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameStringValidator);

        if (nameStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NameValidationFailed)));
        }

        PluginSubscription plugin = this.database.Data.FirstOrDefault(p => p.Key.Equals(key) && p.Name.Equals(name));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        if (plugin.Banned)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginAlreadyBanned)));
        }

        plugin.SetBan();
        this.database.Data.Update(plugin);
        this.database.SaveChanges();

        return Ok();
    }

    public IActionResult BanSubscriptions()
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
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
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        IEnumerable<PluginSubscription> plugins = this.database.Data.Where(p => p.Key.Equals(key));
        if (plugins == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NoOnePluginWithSpecifiedKeyNotFound)));
        }

        foreach (PluginSubscription plugin in plugins)
        {
            plugin.SetBan();
        }

        this.database.Data.UpdateRange(plugins);
        this.database.SaveChanges();

        return Ok();
    }

    public IActionResult UnbanSubscription(string name)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
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
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        PluginSubscription plugin = this.database.Data.FirstOrDefault(p => p.Key.Equals(key) && p.Name.Equals(name));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        plugin.SetUnban();
        this.database.Data.Update(plugin);
        this.database.SaveChanges();

        return Ok();
    }

    public IActionResult UnbanSubscriptions()
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
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
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        IEnumerable<PluginSubscription> plugins = this.database.Data.Where(p => p.Key.Equals(key));
        if (plugins == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NoOnePluginWithSpecifiedKeyNotFound)));
        }

        foreach (PluginSubscription plugin in plugins)
        {
            plugin.SetUnban();
        }

        this.database.Data.UpdateRange(plugins);
        this.database.SaveChanges();

        return Ok();
    }

    public IActionResult GetSubscription(string name)
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
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
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        name.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameStringValidator);

        if (nameStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NameValidationFailed)));
        }

        PluginSubscription plugin = this.database.Data.FirstOrDefault(p => p.Key.Equals(key) && p.Name.Equals(name));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        return Ok(JsonConvert.SerializeObject(plugin));
    }

    public IActionResult GetSubscriptions()
    {
        if (HttpContext.Request.Headers.TryGetValue(KnownHeaders.Key, out StringValues keyStringValue) == false)
        {
            return BadRequest();
        }

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
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
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        return Ok(JsonConvert.SerializeObject(this.database.Data.Where(p => p.Key.Equals(key))));
    }
}
