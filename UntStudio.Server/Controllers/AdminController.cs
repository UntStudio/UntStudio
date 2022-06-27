using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using UntStudio.Server.Data;
using UntStudio.Server.Models;
using UntStudio.Server.Strings;

namespace UntStudio.Server.Controllers;

public sealed class AdminController : ControllerBase
{
    private readonly PluginsDatabaseContext database;



    public AdminController(PluginsDatabaseContext database)
    {
        this.database = database;
    }



    public IActionResult AddSubscription(string name, string key, string allowedAddresses, int days)
    {
        name.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator pluginNameValidator);

        if (pluginNameValidator.Failed)
        {
            return Content("Please, specify plugin name properly!");
        }

        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnowPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyValidator);

        if (keyValidator.Failed)
        {
            return Content("Please, specify plugin key properly!");
        }

        allowedAddresses.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator allowedAddressesValidator);

        if (allowedAddressesValidator.Failed)
        {
            return Content("Please, specify allowed addresses properly!");
        }

        Plugin plugin = new Plugin(name, key, allowedAddresses, DateTime.Now.AddDays(days));
        this.database.Data.Add(plugin);
        this.database.SaveChangesAsync();

        return Content($"Successfuly added new plugin: {plugin}");
    }

    public IActionResult AddFreeSubscription(string name, string key)
    {
        name.Rules()
            .ContentNotNullOrWhiteSpace()
            .Return(out IStringValidator nameValidator);

        if (nameValidator.Failed)
        {
            return Content("Please, specify plugin name properly!");
        }

        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnowPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyValidator);

        if (keyValidator.Failed)
        {
            return Content("Please, specify key properly!");
        }

        Plugin plugin = new Plugin(name, key);
        this.database.Data.Add(plugin);
        this.database.SaveChanges();

        return Content($"Successfuly added new free plugin: {plugin}");
    }

    public IActionResult GetSubscriptionInfo(string key)
    {
        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnowPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content("Please, specify plugin key properly!");
        }

        return Content(this.database.Data.FirstOrDefault(p => p.Key.Equals(key)).ToString());
    }

    public IActionResult GetSubscriptions(string key)
    {
        key.Rules()
            .ContentNotNullOrWhiteSpace()
            .ShouldBeEqualToCharactersLenght(KnowPluginKeyLenghts.Lenght)
            .Return(out IStringValidator keyStringValidator);

        if (keyStringValidator.Failed)
        {
            return Content("Please, specify plugin key properly!");
        }

        return Content(string.Join(", ", this.database.Data.Where(p => p.Key.Equals(key))
            .Select(p => p.ToString())));
    }
}
