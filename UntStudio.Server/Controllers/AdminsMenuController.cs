using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
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

    [HttpGet("addsubscription")]
    public IActionResult AddSubscription()
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

    public IActionResult DownloadDatas(string licenseKey)
    {
        System.Console.WriteLine("License key; " + licenseKey);
        List<PluginSubscription> pluginSubscriptions = this.pluginSubscriptionsDatabase.Data.Where(p => p.LicenseKey.Equals(licenseKey)).ToList();
        System.Console.WriteLine("Count of plugins: " + pluginSubscriptions.Count);
        string serializedData = JsonConvert.SerializeObject(pluginSubscriptions, Formatting.Indented);
        char[] characters = serializedData.ToCharArray();
        byte[] bytes = new byte[characters.Length];
        for (var i = 0; i < characters.Length; i++)
        {
            bytes[i] = (byte)characters[i];
        }

        MemoryStream memoryStream = new MemoryStream();
        memoryStream.Write(bytes);
        memoryStream.Position = 0;

       return File(memoryStream, "APPLICATION/octet-stream", $"{pluginSubscriptions.Count()}_plugins.json");
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
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] = "An error occured on server side, while adding new subscription.";
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
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] = "An error occured on server side, while adding new free subscription.";
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
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] = "An error occured on server side, while banning subscription.";
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
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] = "An error occured on server side, while banning subscriptions.";
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
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] = "An error occured on server side, while unbanning subscription.";
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
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] = "An error occured on server side, while unbanning subscriptions.";
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
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] = "An error occured on server side, while searching for a subscription.";
        }
        catch (JsonReaderException)
        {
            TempData["ErrorMessage"] = "An error occured at json reading, while searching for a subscription.";
        }

        return View();
    }

    [HttpGet("getsubscriptions")]
    public IActionResult GetSubscriptions()
    {
        return View();
    }

    [HttpPost("getsubscriptions")]
    public async Task<IActionResult> GetSubscriptions(string licenseKey)
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

        if (pluginSubscriptionsDatabase.Data.Any(p => p.LicenseKey.Equals(licenseKey)) == false)
        {
            TempData["ErrorMessage"] = "No one subscription cannot be found check your license key.";
            return View();
        }

        try
        {
            System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#1");
            HttpClient httpClient = httpClientFactory.CreateClient(KnownHttpClientNames.AdminsAPI);
            System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#2");

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"GetSubscriptions?{nameof(licenseKey)}={licenseKey}");
            System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#3");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#4");

                string responseText = await httpResponseMessage.Content.ReadAsStringAsync();
                System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#5");

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#6");

                    TempData["ErrorMessage"] = "Something went wrong, no answer from API.";
                    return View();
                }
                System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#7");


                List<PluginSubscription> pluginSubscriptions = null;
                System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#8");

                if ((pluginSubscriptions = JsonConvert.DeserializeObject<List<PluginSubscription>>(responseText)) == null)
                {
                    System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#9");
                    TempData["ErrorMessage"] = "Something went wrong, not able to deserialize response from API to json.";
                    System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#10");
                    return View();
                }
                System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#11");

                TempData["SuccessMessage"] = $"Successfully found subscriptions!";
                System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#12");
                System.Console.WriteLine("Response text START: " + responseText);
                System.Console.WriteLine("Response text END>>:");
                return View(licenseKey);
            }
            System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#13");
        }
        catch (HttpRequestException)
        {
            System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#14");
            TempData["ErrorMessage"] = "An error occured on server side, while searching for a subscriptions.";
            System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#15");
        }
        catch (JsonReaderException)
        {
            System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#16");
            TempData["ErrorMessage"] = "An error occured at json reading, while searching for a subscriptions.";
            System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#17");
        }
        System.Console.WriteLine(">>>>>>>>>>>>>>>>>> FLAG#18");

        return View();
    }
}
