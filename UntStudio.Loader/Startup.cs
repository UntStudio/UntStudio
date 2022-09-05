using Microsoft.Extensions.DependencyInjection;
using SDG.Unturned;
using System;
using UntStudio.Loader.Activators;
using UntStudio.Loader.Decryptors;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Models;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;
using UntStudio.Loader.Solvers;
using static UntStudio.Loader.Models.RequestResponse;

namespace UntStudio.Loader;

public sealed class Startup
{
    public Startup(IServiceProvider serviceProvider)
    {
        initializeAsync(
            serviceProvider.GetRequiredService<ILoaderConfiguration>(),
            serviceProvider.GetRequiredService<IServer>(),
            serviceProvider.GetRequiredService<ILogging>(),
            serviceProvider.GetRequiredService<IDecryptor>(),
            serviceProvider.GetRequiredService<IPEBit>(),
            serviceProvider.GetRequiredService<IRocketModPluginActivator>());
    }



    private async void initializeAsync(ILoaderConfiguration configuration,
        IServer server,
        ILogging logging, 
        IDecryptor decryptor, 
        IPEBit peSolver, 
        IRocketModPluginActivator rocketModPluginActivator)
    {
        for (int i = 0; i < configuration.Plugins.Length; i++)
        {
            ServerResult serverResult = await server.UploadPluginAsync(configuration.LicenseKey, configuration.Plugins[i]);
            if (serverResult.HasResponse)
            {
                logging.LogWarning(translateServerResponse(serverResult.Response.Code));
            }
            if (serverResult.HasBytes)
            {
                try
                {
                    string decryptedContent = await decryptor.DecryptAsync(serverResult.Bytes, configuration.LicenseKey);
                    byte[] bytes = peSolver.Unbit(Convert.FromBase64String(decryptedContent));
                    rocketModPluginActivator.Activate(bytes, configuration.Plugins[i]);

                    PluginAdvertising.Get().AddPlugin(configuration.Plugins[i]);
                    logging.Log($"Plugin {configuration.Plugins[i]} Loaded!", ConsoleColor.Green);
                }
                catch (Exception ex)
                {
                    logging.LogWarning(ex.Message);
                    logging.LogException(ex, $"An not supported error ocurred while loading plugin, please contant with Administrators! Plugin: {configuration.Plugins[i]}.");
                    continue;
                }
            }
        }
    }

    private string translateServerResponse(CodeResponse code)
    {
        return code switch
        {
            CodeResponse.None                                                                  => "Nothing.",
            CodeResponse.VersionOutdated                                                       => "Loader version outdated, please download latest!",
            CodeResponse.LicenseKeyValidationFailed                                            => "Please, check your key, and write it properly!",
            CodeResponse.NameValidationFailed                                                  => "Plugin name validation failed, please verify your plugin configuration.",
            CodeResponse.SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedLicenseKeyNotFound => "Your subscription banned or IP not binded or expired or specified key not found.",
            CodeResponse.SpecifiedLicenseKeyOrIPNotBindedOrNameNotFound                        => "Your key is not binded or key does not exist or plugin name not found.",
            CodeResponse.SubscriptionBanned                                                    => "Your subscription was banned.",
            CodeResponse.SubscriptionExpired                                                   => "Your subscription was expired.",
            CodeResponse.SubscriptionBlockedByOwner                                            => "Your subscription was blocked by yourself, and cannot be used.",
            CodeResponse.SubscriptionAlreadyBlocked                                            => "Your subscription was already blocked by yourself.",
            CodeResponse.SubscriptionAlreadyUnblocked                                          => "Your subscription was already unblocked by yourself.",
            _ => "Unknown server response, please contact with Administrator, may version is outdated.",
        };
    }
}
