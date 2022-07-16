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

[AllowUnauthenticatedHost(KnownHosts.DeployedWebsiteURL)]
public sealed class AdminController : ControllerBase
{
    private readonly PluginSubscriptionsDatabaseContext pluginsDatabase;



    public AdminController(PluginSubscriptionsDatabaseContext pluginsDatabase)
    {
        this.pluginsDatabase = pluginsDatabase;
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

        PluginSubscription plugin = new PluginSubscription(pluginName, licenseKey, allowedAddresses, DateTime.Now.AddDays(days));
        this.pluginsDatabase.Data.Add(plugin);
        await this.pluginsDatabase.SaveChangesAsync();

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

        PluginSubscription plugin = new PluginSubscription(pluginName, licenseKey, allowedAddresses);
        plugin.SetFree();
        this.pluginsDatabase.Data.Add(plugin);
        await this.pluginsDatabase.SaveChangesAsync();

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

        PluginSubscription plugin = this.pluginsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        if (plugin.Banned)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginAlreadyBanned)));
        }

        plugin.SetBan();
        this.pluginsDatabase.Data.Update(plugin);
        await this.pluginsDatabase.SaveChangesAsync();

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

        IEnumerable<PluginSubscription> plugins = this.pluginsDatabase.Data.Where(p => p.LicenseKey.Equals(licenseKey));
        if (plugins == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NoOnePluginWithSpecifiedKeyNotFound)));
        }

        foreach (PluginSubscription plugin in plugins)
        {
            plugin.SetBan();
        }

        this.pluginsDatabase.Data.UpdateRange(plugins);
        await this.pluginsDatabase.SaveChangesAsync();

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

        PluginSubscription plugin = this.pluginsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        plugin.SetUnban();
        this.pluginsDatabase.Data.Update(plugin);
        await this.pluginsDatabase.SaveChangesAsync();

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

        IEnumerable<PluginSubscription> plugins = this.pluginsDatabase.Data.Where(p => p.LicenseKey.Equals(licenseKey));
        if (plugins == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.NoOnePluginWithSpecifiedKeyNotFound)));
        }

        foreach (PluginSubscription plugin in plugins)
        {
            plugin.SetUnban();
        }

        this.pluginsDatabase.Data.UpdateRange(plugins);
        await this.pluginsDatabase.SaveChangesAsync();

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

        PluginSubscription plugin = this.pluginsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName));
        if (plugin == null)
        {
            return Content(JsonConvert.SerializeObject(new AdminRequestResponse(AdminCodeResponse.SpecifiedPluginKeyOrNameNotFound)));
        }

        return Ok(JsonConvert.SerializeObject(plugin));
    }

    public IActionResult GetSubscriptions(string licenseKey)
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

        return Ok(JsonConvert.SerializeObject(this.pluginsDatabase.Data.Where(p => p.LicenseKey.Equals(licenseKey)).ToList(), Formatting.Indented));
    }

    public IActionResult GetAllSubscriptions()
    {
        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues userAgentStringValue) == false)
        {
            return BadRequest();
        }

        if (userAgentStringValue != KnownHeaders.UserAgentAdminValue)
        {
            return BadRequest();
        }

        return Ok(JsonConvert.SerializeObject(this.pluginsDatabase.Data.ToList()));
    }
}
