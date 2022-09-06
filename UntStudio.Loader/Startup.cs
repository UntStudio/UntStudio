using Microsoft.Extensions.DependencyInjection;
using SDG.Unturned;
using System;
using System.Linq;
using System.Reflection;
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
            serviceProvider.GetRequiredService<IMonoActivator>(),
            serviceProvider.GetRequiredService<IPluginFrameworkActivatorResolver>());
    }



    private async void initializeAsync(ILoaderConfiguration configuration,
        IServer server,
        ILogging logging, 
        IDecryptor decryptor, 
        IPEBit peSolver,
        IMonoActivator monoActivator,
        IPluginFrameworkActivatorResolver pluginFrameworkActivatorResolver)
    {
        for (int i = 0; i < configuration.Plugins.Length; i++)
        {
            string pluginName = configuration.Plugins[i];
            ServerResult serverResult = await server.UploadPluginAsync(configuration.LicenseKey, pluginName);
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
                    IntPtr assemblyHandle = monoActivator.Activate(bytes);
                    Assembly pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.GetName().Name.Equals(pluginName));
                    pluginFrameworkActivatorResolver.Resolve(pluginAssembly).Activate(assemblyHandle, pluginAssembly);

                    //rocketModPluginActivator.Activate(bytes, configuration.Plugins[i]);

                    PluginAdvertising.Get().AddPlugin(pluginName);
                    logging.Log($"Plugin {pluginName} Loaded!", ConsoleColor.Green);
                }
                catch (UnsupportedPluginFrameworkException)
                {
                    logging.LogWarning($"{pluginName} failed to load! Unsupported plugin framework.");
                    continue;
                }
                catch (Exception ex)
                {
                    logging.LogWarning(ex.Message);
                    logging.LogException(ex, $"An not supported error ocurred while loading plugin, please contant with Administrators! Plugin: {pluginName}.");
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
