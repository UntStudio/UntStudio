using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UntStudio.Server.Attributes;
using UntStudio.Server.Data;
using UntStudio.Server.Knowns;
using UntStudio.Server.Models;
using UntStudio.Server.Strings;
using static UntStudio.Server.Models.AdminRequestResponse;

namespace UntStudio.Server.Controllers;

//[AllowUnauthenticatedHost(KnownHosts.DeployedWebsiteURL)]
public sealed class AdminController : ControllerBase
{
    private readonly PluginSubscriptionsDatabaseContext subscriptionsDatabase;



    public AdminController(PluginSubscriptionsDatabaseContext pluginsDatabase)
    {
        this.subscriptionsDatabase = pluginsDatabase;
    }



    public async Task<IActionResult> AddSubscription(string licenseKey, string pluginName, string allowedAddresses, int days)
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        licenseKey.Rules()
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

        if (this.subscriptionsDatabase.Data.FirstOrDefault(s => s.LicenseKey.Equals(licenseKey) && s.Name.Equals(pluginName)) != null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SubscriptionAlreadyExist)));
        }

        PluginSubscription plugin = new PluginSubscription(pluginName, licenseKey, allowedAddresses, DateTime.Now.AddDays(days));
        this.subscriptionsDatabase.Data.Add(plugin);
        await this.subscriptionsDatabase.SaveChangesAsync();

        return Ok(JsonConvert.SerializeObject(plugin));
    }

    public async Task<IActionResult> AddFreeSubscription(string licenseKey, string pluginName, string allowedAddresses)
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        pluginName.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameStringValidator);

        if (nameStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NameValidationFailed)));
        }

        if (this.subscriptionsDatabase.Data.FirstOrDefault(s => s.LicenseKey.Equals(licenseKey) && s.Name.Equals(pluginName)) != null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SubscriptionAlreadyExist)));
        }

        PluginSubscription plugin = new PluginSubscription(pluginName, licenseKey, allowedAddresses);
        plugin.SetFree();
        this.subscriptionsDatabase.Data.Add(plugin);
        await this.subscriptionsDatabase.SaveChangesAsync();

        return Ok(JsonConvert.SerializeObject(plugin));
    }

    public async Task<IActionResult> BanSubscription(string licenseKey, string pluginName)
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        pluginName.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameStringValidator);

        if (nameStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NameValidationFailed)));
        }

        PluginSubscription plugin = null;
        if ((plugin = this.subscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName))) == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        if (plugin.Banned)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginAlreadyBanned)));
        }

        plugin.SetBan();
        this.subscriptionsDatabase.Data.Update(plugin);
        await this.subscriptionsDatabase.SaveChangesAsync();

        return Ok();
    }

    public async Task<IActionResult> BanSubscriptions(string licenseKey)
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        IEnumerable<PluginSubscription> plugins = null;
        if ((plugins = this.subscriptionsDatabase.Data.Where(p => p.LicenseKey.Equals(licenseKey))) == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NoOnePluginWithSpecifiedKeyNotFound)));
        }

        foreach (PluginSubscription plugin in plugins)
        {
            plugin.SetBan();
        }

        this.subscriptionsDatabase.Data.UpdateRange(plugins);
        await this.subscriptionsDatabase.SaveChangesAsync();

        return Ok();
    }

    public async Task<IActionResult> UnbanSubscription(string licenseKey, string pluginName)
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        PluginSubscription subscription = null;
        if ((subscription = this.subscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName))) == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        subscription.SetUnban();
        this.subscriptionsDatabase.Data.Update(subscription);
        await this.subscriptionsDatabase.SaveChangesAsync();

        return Ok();
    }

    public async Task<IActionResult> UnbanSubscriptions(string licenseKey)
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        IEnumerable<PluginSubscription> plugins = null;
        if ((plugins = this.subscriptionsDatabase.Data.Where(p => p.LicenseKey.Equals(licenseKey))) == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NoOnePluginWithSpecifiedKeyNotFound)));
        }

        foreach (PluginSubscription plugin in plugins)
        {
            plugin.SetUnban();
        }

        this.subscriptionsDatabase.Data.UpdateRange(plugins);
        await this.subscriptionsDatabase.SaveChangesAsync();

        return Ok();
    }

    public IActionResult GetSubscription(string licenseKey, string pluginName)
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        pluginName.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameStringValidator);

        if (nameStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NameValidationFailed), Formatting.Indented));
        }

        PluginSubscription subscription = null;
        if ((subscription = this.subscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName))) == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        return Ok(JsonConvert.SerializeObject(subscription));
    }

    public async Task<IActionResult> UpdateSubscriptionIP(string licenseKey, string pluginName, string newIP)
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        PluginSubscription subscription = null;
        if ((subscription = this.subscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName))) == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        subscription.AllowedAddresses = newIP;
        this.subscriptionsDatabase.Data.Update(subscription);
        await this.subscriptionsDatabase.SaveChangesAsync();
        return Ok();
    }

    public async Task<IActionResult> AddSubscriptionDays(string licenseKey, string pluginName, int newDays)
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        PluginSubscription subscription = null;
        if ((subscription = this.subscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName))) == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        subscription.ExpirationTime = subscription.ExpirationTime.AddDays(newDays);
        this.subscriptionsDatabase.Data.Update(subscription);
        await this.subscriptionsDatabase.SaveChangesAsync();
        return Ok();
    }

    public async Task<IActionResult> SetSubscriptionDays(string licenseKey, string pluginName, int newDays)
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.KeyValidationFailed)));
        }

        PluginSubscription subscription = null;
        if ((subscription = this.subscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName))) == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        subscription.ExpirationTime = DateTime.Now.AddDays(newDays);
        this.subscriptionsDatabase.Data.Update(subscription);
        await this.subscriptionsDatabase.SaveChangesAsync();
        return Ok();
    }
}
