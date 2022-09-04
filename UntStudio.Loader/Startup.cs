using Microsoft.Extensions.DependencyInjection;
using SDG.Unturned;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UntStudio.Loader.Models;
using UntStudio.Loader.Decryptors;
using UntStudio.Loader.External;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;
using static UntStudio.Loader.Models.RequestResponse;
using Object = UnityEngine.Object;
using UntStudio.Loader.Solvers;

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
            serviceProvider.GetRequiredService<IPESolver>());
    }



    private async void initializeAsync(ILoaderConfiguration configuration, IServer server, ILogging logging, IDecryptor decryptor, IPESolver peSolver)
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
                    byte[] bytes = peSolver.Solve(Convert.FromBase64String(decryptedContent));
                    unsafe
                    {
                        fixed (byte* pointer = bytes)
                        {
                            IntPtr imageHandle = ExternalMonoCalls.MonoImageOpenFromData((IntPtr)pointer, bytes.Length, false, out _);
                            IntPtr assemblyHandle = ExternalMonoCalls.MonoAssemblyLoadFrom(imageHandle, string.Empty, out _);

                            GameObject containerGameObject = new GameObject();
                            MethodInfo createGameObjectMethodInfo = typeof(GameObject).GetMethod("Internal_CreateGameObject",
                                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
                            MethodInfo addComponentMethodInfo = typeof(GameObject).GetMethod("Internal_AddComponentWithType",
                                BindingFlags.Instance | BindingFlags.NonPublic);

                            createGameObjectMethodInfo.Invoke(null, new object[]
                            {
                                containerGameObject,
                                configuration.Plugins[i],
                            });

                            Assembly pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.GetName().Name.Equals(configuration.Plugins[i]));
                            if (pluginAssembly == null)
                            {
                                logging.LogWarning($"Cannot find plugin {configuration.Plugins[i]}.");
                                continue;
                            }

                            Type pluginType = pluginAssembly.GetTypes().SingleOrDefault(t => t.GetInterface("IRocketPlugin") != null);
                            if (pluginType == null)
                            {
                                logging.LogWarning($"Given plugin from license server is outdated {configuration.Plugins[i]}.");
                                continue;
                            }

                            addComponentMethodInfo.Invoke(containerGameObject, new object[]
                            {
                                pluginType,
                            });

                            Object.DontDestroyOnLoad(containerGameObject);
                            PluginAdvertising.Get().AddPlugin(configuration.Plugins[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
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
