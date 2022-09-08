using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UntStudio.Server.Bits;
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
    private readonly IPEBit peBit;

    public PluginSubscriptionsController(PluginSubscriptionsDatabaseContext database, IConfiguration configuration, IEncryptor encryptor, IPEBit peResolver)
    {
        this.database = database;
        this.configuration = configuration;
        this.encryptor = encryptor;
        this.peBit = peResolver;
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
        byte[] bytes = null;
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
            bytes = System.IO.File.ReadAllBytes(pluginFile);
            bytes = this.peBit.Bit(bytes);
            bytes = await this.encryptor.EncryptContentAsync(Convert.ToBase64String(bytes), licenseKey);

            return Ok(Convert.ToBase64String(bytes));
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
        bytes = System.IO.File.ReadAllBytes(pluginFile);
        bytes = this.peBit.Bit(bytes);
        bytes = await this.encryptor.EncryptContentAsync(Convert.ToBase64String(bytes), licenseKey);

        return Ok(Convert.ToBase64String(bytes));
    }
}
