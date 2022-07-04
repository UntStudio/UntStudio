using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UntStudio.Bootstrapper.API;
using UntStudio.Bootstrapper.Loaders;
using UntStudio.Bootstrapper.Models;
using static UntStudio.Bootstrapper.API.RequestResponse;

namespace UntStudio.Bootstrapper
{
    internal sealed class Startup : RocketPlugin<BootstrapperConfiguration>
    {
        protected override async void Load()
        {
            try
            {
                IBootstrapper bootstrapper = new Bootstrapper();
                ServerResult loaderServerResult = await bootstrapper.GetUnloadLoaderAsync(Configuration.Instance.Key);

                if (loaderServerResult == null)
                {
                    Rocket.Core.Logging.Logger.Log("Cannot load loader!");
                    return;
                }

                if (loaderServerResult.HasResponse)
                {
                    Rocket.Core.Logging.Logger.LogWarning("You have a new response from server!");
                    Rocket.Core.Logging.Logger.LogWarning(translateServerResponse(loaderServerResult.Response.Code));
                    return;
                }

                ServerResult loaderEntryPointServerResult = await bootstrapper.GetLoaderEntryPointAsync(Configuration.Instance.Key);

                if (loaderEntryPointServerResult == null)
                {
                    Rocket.Core.Logging.Logger.Log("Cannot load loader!");
                    return;
                }

                if (loaderEntryPointServerResult.HasResponse)
                {
                    Rocket.Core.Logging.Logger.LogWarning("You have a new response from server!");
                    Rocket.Core.Logging.Logger.LogWarning(translateServerResponse(loaderEntryPointServerResult.Response.Code));
                    return;
                }

                if (loaderEntryPointServerResult.HasLoaderEntryPoint == false)
                {
                    Rocket.Core.Logging.Logger.Log("Cannot load loader!");
                    return;
                }

                if (loaderServerResult.HasBytes)
                {
                    unsafe
                    {
                        fixed (byte* pointer = loaderServerResult.Bytes)
                        {
                            IntPtr imageHandle = ExternalMonoCalls.MonoImageOpenFromData((IntPtr)pointer, loaderServerResult.Bytes.Length, false, out _);
                            ExternalMonoCalls.MonoAssemblyLoadFrom(imageHandle, string.Empty, out _);

                            IntPtr classHandle = ExternalMonoCalls.MonoClassFromName(imageHandle,
                                loaderEntryPointServerResult.LoaderEntryPoint.Namespace,
                                loaderEntryPointServerResult.LoaderEntryPoint.Class);

                            IntPtr methodHandle = ExternalMonoCalls.MonoClassGetMethodFromName(classHandle,
                                loaderEntryPointServerResult.LoaderEntryPoint.Method, 1);

                            string pluginsFormatted = string.Join(",", Configuration.Instance.UntStudioPlugins.Select(p => p.Name));
                            string formattedShowPluginKeyPluginsText = $"{Configuration.Instance.DisplayPluginsInServerPluginsMenu};{Configuration.Instance.Key};{pluginsFormatted}";
                            string[] array = new string[]
                            {
                                formattedShowPluginKeyPluginsText,
                            };

                            IntPtr arrayHandle = Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
                            ExternalMonoCalls.MonoRuntimeInvoke(methodHandle, IntPtr.Zero, arrayHandle, IntPtr.Zero);
                        }
                    }

                    if (Configuration.Instance.DisplayLoaderInServerPluginsMenu)
                    {
                        PluginAdvertising.Get().AddPlugin(typeof(Startup).Namespace);
                    }

                    if (Configuration.Instance.DisplayPluginsInServerPluginsMenu)
                    {
                        foreach (UntStudioPlugin plugin in Configuration.Instance.UntStudioPlugins.Where(p => p.Enabled))
                        {
                            PluginAdvertising.Get().AddPlugin(plugin.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error ocurred while loading bootsrapper!");
            }
        }



        private string translateServerResponse(CodeResponse code)
        {
            return code switch
            {
                CodeResponse.None                                                            => "Nothing.",
                CodeResponse.VersionOutdated                                                 => "Loader version outdated, please download latest!",
                CodeResponse.KeyValidationFailed                                             => "Please, check your key, and write it properly!",
                CodeResponse.NameValidationFailed                                            => "Plugin name validation failed, please verify your plugin configuration.",
                CodeResponse.SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedKeyNotFound  => "Your subscription banned or IP not binded or expired or specified key not found.",
                CodeResponse.SpecifiedKeyOrIPNotBindedOrNameNotFound                         => "Your key is not binded or key does not exist or plugin name not found.",
                CodeResponse.SubscriptionBanned                                              => "Your subscription was banned.",
                CodeResponse.SubscriptionExpired                                             => "Your subscription was expired.",
                CodeResponse.SubscriptionBlockedByOwner                                      => "Your subscription was blocked by yourself, and cannot be used.",
                CodeResponse.SubscriptionAlreadyBlocked                                      => "Your subscription was already blocked by yourself.",
                CodeResponse.SubscriptionAlreadyUnblocked                                    => "Your subscription was already unblocked by yourself.",
                _ => "Unknown server response, please contact with Administrator, may version is outdated.",
            };
        }
    }
}
