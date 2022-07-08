using SDG.Unturned;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UntStudio.Loader.API;
using UntStudio.Loader.External;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;
using static UntStudio.Loader.API.RequestResponse;
using Object = UnityEngine.Object;

namespace UntStudio.Loader;

public sealed class Startup
{
    public Startup(IServiceProvider serviceProvider)
    {
        initializeAsync
        (
            (ILoaderConfiguration)serviceProvider.GetService(typeof(ILoaderConfiguration)),
            (IServer)serviceProvider.GetService(typeof(IServer)),
            (ILogging)serviceProvider.GetService(typeof(ILogging))
        );
    }



    private async void initializeAsync(ILoaderConfiguration configuration, IServer server, ILogging logging)
    {
        for (int i = 0; i < configuration.Plugins.Length; i++)
        {
            ServerResult serverResult = await server.GetUnloadPluginAsync(configuration.LicenseKey, configuration.Plugins[i]);
            if (serverResult.HasResponse)
            {
                translateServerResponse(serverResult.Response.Code);
            }
            if (serverResult.HasBytes)
            {
                try
                {
                    unsafe
                    {
                        fixed (byte* pointer = serverResult.Bytes)
                        {
                            IntPtr imageHandle = ExternalMonoCalls.MonoImageOpenFromData((IntPtr)pointer, serverResult.Bytes.Length, false, out _);
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

                            Assembly pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name.Equals(configuration.Plugins[i]));
                            if (pluginAssembly == null)
                            {
                                logging.Log($"Cannot find plugin {configuration.Plugins[i]}.");
                                continue;
                            }

                            Type pluginType = pluginAssembly.GetTypes().First(t => t.GetInterface("IRocketPlugin") != null);
                            if (pluginType == null)
                            {
                                logging.Log($"Given plugin from license server is outdated {configuration.Plugins[i]}.");
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
