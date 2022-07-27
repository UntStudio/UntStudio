using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UntStudio.Server.Data;
using UntStudio.Server.Knowns;
using UntStudio.Server.Models;
using UntStudio.Server.Strings;

namespace UntStudio.Server.Controllers;

[Authorize]
public sealed class AdminsMenuController : Controller
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly PluginSubscriptionsDatabaseContext pluginSubscriptionsDatabase;



    public AdminsMenuController(IHttpClientFactory httpClientFactory, PluginSubscriptionsDatabaseContext pluginSubscriptionsDatabase)
    {
        this.httpClientFactory = httpClientFactory;
        this.pluginSubscriptionsDatabase = pluginSubscriptionsDatabase;
    }



    [HttpGet("help")]
    public IActionResult Help()
    {
        return View();
    }

    public IActionResult DownloadData(PluginSubscription pluginSubscription)
    {
        string serializedData = JsonConvert.SerializeObject(pluginSubscription, Formatting.Indented);
        char[] characters = serializedData.ToCharArray();
        byte[] bytes = new byte[characters.Length];
        for (var i = 0; i < characters.Length; i++)
        {
            bytes[i] = (byte)characters[i];
        }

        MemoryStream memoryStream = new MemoryStream();
        memoryStream.Write(bytes);
        memoryStream.Position = 0;

        return File(memoryStream, "APPLICATION/octet-stream", $"{pluginSubscription.Name}.json");
    }

    [HttpGet("addsubscription")]
    public IActionResult AddSubscription()
    {
        return View();
    }

    [HttpPost("addsubscription")]
    public async Task<IActionResult> AddSubscription(string licenseKey, string pluginName, string allowedAddresses, int days)
    {
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            TempData["ErrorMessage"] = "License key cannot be empty or less than 19 characters.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(pluginName))
        {
            TempData["ErrorMessage"] = "Plugin name cannot be empty.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(allowedAddresses))
        {
            TempData["ErrorMessage"] = "Allowed Addresses cannot be empty.";
            return View();
        }

        if (days == 0)
        {
            TempData["ErrorMessage"] = "Days cannot be specified as zero.";
            return View();
        }

        try
        {
            HttpClient httpClient = httpClientFactory.CreateClient(KnownHttpClientNames.AdminsAPI);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"AddSubscription?{nameof(licenseKey)}={licenseKey}&{nameof(pluginName)}={pluginName}&{nameof(allowedAddresses)}={allowedAddresses}&{nameof(days)}={days}", null);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Successfully added new subscription!";
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = $"[{ex.StatusCode}] An error occured on server side, while adding new subscription.";
        }

        return View();
    }

    [HttpGet("addfreesubscription")]
    public IActionResult AddFreeSubscription()
    {
        return View();
    }

    [HttpPost("addfreesubscription")]
    public async Task<IActionResult> AddFreeSubscription(string licenseKey, string pluginName, string allowedAddresses)
    {
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            TempData["ErrorMessage"] = "License key cannot be empty or less than 19 characters.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(pluginName))
        {
            TempData["ErrorMessage"] = "Plugin name cannot be empty.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(allowedAddresses))
        {
            TempData["ErrorMessage"] = "Allowed Addresses cannot be empty.";
            return View();
        }

        try
        {
            HttpClient httpClient = httpClientFactory.CreateClient(KnownHttpClientNames.AdminsAPI);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"AddFreeSubscription?{nameof(licenseKey)}={licenseKey}&{nameof(pluginName)}={pluginName}&{nameof(allowedAddresses)}={allowedAddresses}", null);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Successfully added new free subscription!";
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = $"[{ex.StatusCode}] An error occured on server side, while adding new free subscription.";
        }

        return View();
    }

    [HttpGet("bansubscription")]
    public IActionResult BanSubscription()
    {
        return View();
    }

    [HttpPost("bansubscription")]
    public async Task<IActionResult> BanSubscription(string licenseKey, string pluginName)
    {
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            TempData["ErrorMessage"] = "License key cannot be empty or less than 19 characters.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(pluginName))
        {
            TempData["ErrorMessage"] = "Plugin name cannot be empty.";
            return View();
        }

        PluginSubscription subscription = null;
        if ((subscription = pluginSubscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName))) == null)
        {
            TempData["ErrorMessage"] = "Specified subscription cannot be found, check your license key or plugin name.";
            return View();
        }

        if (subscription.Banned)
        {
            TempData["ErrorMessage"] = "This subscription is already were banned.";
            return View();
        }

        try
        {
            HttpClient httpClient = httpClientFactory.CreateClient(KnownHttpClientNames.AdminsAPI);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"BanSubscription?{nameof(licenseKey)}={licenseKey}&{nameof(pluginName)}={pluginName}", null);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Successfully banned subscription!";
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = $"[{ex.StatusCode}] An error occured on server side, while banning subscription.";
        }

        return View();
    }

    [HttpGet("bansubscriptions")]
    public IActionResult BanSubscriptions()
    {
        return View();
    }

    [HttpPost("bansubscriptions")]
    public async Task<IActionResult> BanSubscriptions(string licenseKey)
    {
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            TempData["ErrorMessage"] = "License key cannot be empty or less than 19 characters.";
            return View();
        }

        int subscriptionsCount = 0;
        if ((subscriptionsCount = pluginSubscriptionsDatabase.Data.Count(p => p.LicenseKey.Equals(licenseKey))) == 0)
        {
            TempData["ErrorMessage"] = "Specified subscription cannot be found or check your license key.";
            return View();
        }

        try
        {
            HttpClient httpClient = httpClientFactory.CreateClient(KnownHttpClientNames.AdminsAPI);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"BanSubscriptions?{nameof(licenseKey)}={licenseKey}", null);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Successfully banned {subscriptionsCount} subscriptions!";
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = $"[{ex.StatusCode}] An error occured on server side, while banning subscriptions.";
        }

        return View();
    }

    [HttpGet("unbansubscription")]
    public IActionResult UnbanSubscription()
    {
        return View();
    }

    [HttpPost("unbansubscription")]
    public async Task<IActionResult> UnbanSubscription(string licenseKey, string pluginName)
    {
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            TempData["ErrorMessage"] = "License key cannot be empty or less than 19 characters.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(pluginName))
        {
            TempData["ErrorMessage"] = "Plugin name cannot be empty.";
            return View();
        }

        if (pluginSubscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName)) == null)
        {
            TempData["ErrorMessage"] = "Specified subscription cannot be found or check your license key.";
            return View();
        }

        try
        {
            HttpClient httpClient = httpClientFactory.CreateClient(KnownHttpClientNames.AdminsAPI);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"UnbanSubscription?{nameof(licenseKey)}={licenseKey}&{nameof(pluginName)}={pluginName}", null);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Successfully unbanned subscription!";
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = $"[{ex.StatusCode}] An error occured on server side, while unbanning subscription.";
        }

        return View();
    }

    [HttpGet("unbansubscriptions")]
    public IActionResult UnbanSubscriptions()
    {
        return View();
    }

    [HttpPost("unbansubscriptions")]
    public async Task<IActionResult> UnbanSubscriptions(string licenseKey)
    {
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            TempData["ErrorMessage"] = "License key cannot be empty or less than 19 characters.";
            return View();
        }

        int subscriptionsCount = 0;
        if ((subscriptionsCount = pluginSubscriptionsDatabase.Data.Count(p => p.LicenseKey.Equals(licenseKey))) == 0)
        {
            TempData["ErrorMessage"] = "No one subscription cannot be found check your license key.";
            return View();
        }

        try
        {
            HttpClient httpClient = httpClientFactory.CreateClient(KnownHttpClientNames.AdminsAPI);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"UnbanSubscriptions?{nameof(licenseKey)}={licenseKey}", null);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Successfully unbanned {subscriptionsCount} subscriptions!";
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = $"[{ex.StatusCode}] An error occured on server side, while unbanning subscriptions.";
        }

        return View();
    }

    [HttpGet("getsubscription")]
    public IActionResult GetSubscription()
    {
        return View();
    }

    [HttpPost("getsubscription")]
    public async Task<IActionResult> GetSubscription(string licenseKey, string pluginName)
    {
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            TempData["ErrorMessage"] = "License key cannot be empty or less than 19 characters.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(pluginName))
        {
            TempData["ErrorMessage"] = "Plugin name cannot be empty.";
            return View();
        }

        if (pluginSubscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName)) == null)
        {
            TempData["ErrorMessage"] = "Specified subscription cannot be found check your license key or plugin name.";
            return View();
        }

        try
        {
            HttpClient httpClient = httpClientFactory.CreateClient(KnownHttpClientNames.AdminsAPI);
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"GetSubscription?{nameof(licenseKey)}={licenseKey}&{nameof(pluginName)}={pluginName}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string responseText = await httpResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseText))
                {
                    TempData["ErrorMessage"] = "Something went wrong, no answer from API.";
                    return View();
                }

                PluginSubscription pluginSubscription = null;
                if ((pluginSubscription = JsonConvert.DeserializeObject<PluginSubscription>(responseText)) == null)
                {
                    TempData["ErrorMessage"] = "Something went wrong, not able to deserialize response from API to json.";
                    return View();
                }

                TempData["SuccessMessage"] = $"Successfully found subscription!";
                return View(pluginSubscription);
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = $"[{ex.StatusCode}] An error occured on server side, while searching for a subscription.";
        }
        catch (JsonReaderException)
        {
            TempData["ErrorMessage"] = "An error occured at json reading, while searching for a subscription.";
        }

        return View();
    }

    [HttpGet("updatesubscriptionip")]
    public IActionResult UpdateSubscriptionIP()
    {
        return View();
    }
    
    [HttpPost("updatesubscriptionip")]
    public async Task<IActionResult> UpdateSubscriptionIP(string licenseKey, string pluginName, string newIP)
    {
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            TempData["ErrorMessage"] = "License key cannot be empty or less than 19 characters.";
            return View();
        }

        PluginSubscription subscription = null;
        if ((subscription = pluginSubscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName))) == null)
        {
            TempData["ErrorMessage"] = "Specified subscription cannot be found or check your license key or plugin name.";
            return View();
        }

        if (string.IsNullOrEmpty(newIP))
        {
            TempData["WarningMessage"] = "Subscription successfully found! Please, now update the IP.";
            return View(subscription);
        }

        if (newIP.Equals(subscription.AllowedAddresses))
        {
            TempData["SuccessMessage"] = "You didnt made any changes in IP.";
            return View(subscription);
        }

        try
        {
            HttpClient httpClient = httpClientFactory.CreateClient(KnownHttpClientNames.AdminsAPI);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"UpdateSubscriptionIP?{nameof(licenseKey)}={licenseKey}&{nameof(pluginName)}={pluginName}&{nameof(newIP)}={newIP}", null);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Successfully updated subscription IP!";
            }
            else
            {
                TempData["ErrorMessage"] = $"[{httpResponseMessage.StatusCode}] Failed to update subscription IP!";
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = $"[{ex.StatusCode}] An error occured on server side, while updating subscription IP.";
        }

        return View();
    }

   /* [HttpPost("updatesubscriptionexprire")]
    public IActionResult UpdateSubscriptionExpire()
    {
        return View();
    }

    [HttpGet("updatesubscriptionexpire")]
    public async Task<IActionResult> UpdateSubscriptionExpire(string licenseKey, string pluginName, int newDays)
    {
        licenseKey.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnownPluginKeyLenghts.Lenght)
            .Return(out IStringValidator licenseKeyStringValidator);

        if (licenseKeyStringValidator.Failed)
        {
            TempData["ErrorMessage"] = "License key cannot be empty or less than 19 characters.";
            return View();
        }

        PluginSubscription subscription = null;
        if ((subscription = pluginSubscriptionsDatabase.Data.FirstOrDefault(p => p.LicenseKey.Equals(licenseKey) && p.Name.Equals(pluginName))) == null)
        {
            TempData["ErrorMessage"] = "Specified subscription cannot be found or check your license key or plugin name.";
            return View();
        }

        if (string.IsNullOrEmpty(newIP))
        {
            TempData["WarningMessage"] = "Subscription successfully found! Please, now update the IP.";
            return View(subscription);
        }

        if (newIP.Equals(subscription.AllowedAddresses))
        {
            TempData["SuccessMessage"] = "You didnt made any changes in IP.";
            return View(subscription);
        }

        try
        {
            HttpClient httpClient = httpClientFactory.CreateClient(KnownHttpClientNames.AdminsAPI);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"UpdateSubscriptionExpire?{nameof(licenseKey)}={licenseKey}&{nameof(pluginName)}={pluginName}&{nameof(newIP)}={newIP}", null);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Successfully updated subscription IP!";
            }
            else
            {
                TempData["ErrorMessage"] = $"[{httpResponseMessage.StatusCode}] Failed to update subscription IP!";
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = $"[{ex.StatusCode}] An error occured on server side, while updating subscription IP.";
        }

        return View();
    }*/
}
