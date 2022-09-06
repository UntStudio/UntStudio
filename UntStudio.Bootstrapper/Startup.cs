using Newtonsoft.Json;
using Rocket.Core;
using Rocket.Core.Plugins;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UntStudio.API.Bootstrapper.Models;
using UntStudio.Bootstrapper.API;
using UntStudio.Bootstrapper.External;
using UntStudio.Bootstrapper.Models;
using static UntStudio.API.Bootstrapper.Models.RequestResponse;

namespace UntStudio.Bootstrapper
{
    internal sealed class Startup : RocketPlugin
    {
        protected override void Load()
        {
            R.Plugins.OnPluginsLoaded += onPluginsLoaded;
        }

        protected override void Unload()
        {
            R.Plugins.OnPluginsLoaded -= onPluginsLoaded;
        }


        private string translateServerResponse(CodeResponse code)
        {
            return code switch
            {
                CodeResponse.None                                                                  => "Nothing.",
                CodeResponse.VersionOutdated                                                       => "Loader version outdated, please download latest!",
                CodeResponse.LicenseKeyValidationFailed                                            => "Please, check your license key, and write it properly!",
                CodeResponse.NameValidationFailed                                                  => "Plugin name validation failed, please verify your plugin configuration.",
                CodeResponse.SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedLicenseKeyNotFound => "Your subscription banned or IP not binded or expired or specified license key not found.",
                CodeResponse.SpecifiedLicenseKeyOrIPNotBindedOrNameNotFound                        => "Your license key is not binded or license key does not exist or plugin name not found.",
                CodeResponse.SubscriptionBanned                                                    => "Your subscription was banned.",
                CodeResponse.SubscriptionExpired                                                   => "Your subscription was expired.",
                CodeResponse.SubscriptionBlockedByOwner                                            => "Your subscription was blocked by yourself, and cannot be used.",
                CodeResponse.SubscriptionAlreadyBlocked                                            => "Your subscription was already blocked by yourself.",
                CodeResponse.SubscriptionAlreadyUnblocked                                          => "Your subscription was already unblocked by yourself.",
                _ => "Unknown server response, please contact with Administrator, may version is outdated.",
            };
        }

        private async void onPluginsLoaded()
        {
            try
            {
                Configuration configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(Path.Combine(
                    Rocket.Core.Environment.PluginsDirectory,
                    typeof(Startup).Namespace,
                    "config.json")));
                IBootstrapper bootstrapper = new Bootstrapper();
                ServerResult loaderServerResult = await bootstrapper.UploadLoaderAsync(configuration.LicenseKey);

                if (loaderServerResult == null)
                {
                    Rocket.Core.Logging.Logger.LogWarning("Cannot load loader #1!");
                    return;
                }

                if (loaderServerResult.HasResponse)
                {
                    Rocket.Core.Logging.Logger.LogWarning(translateServerResponse(loaderServerResult.Response.Code));
                    return;
                }

                ServerResult loaderEntryPointServerResult = await bootstrapper.GetLoaderEntryPointAsync(configuration.LicenseKey);
                if (loaderEntryPointServerResult == null)
                {
                    Rocket.Core.Logging.Logger.LogWarning("Cannot load loader #2!");
                    return;
                }

                if (loaderEntryPointServerResult.HasResponse)
                {
                    Rocket.Core.Logging.Logger.LogWarning(translateServerResponse(loaderEntryPointServerResult.Response.Code));
                    return;
                }

                if (loaderEntryPointServerResult.HasLoaderEntryPoint == false)
                {
                    Rocket.Core.Logging.Logger.LogWarning("Cannot load loader #3!");
                    return;
                }

                /*if (AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains(loaderEntryPointServerResult.LoaderEntryPoint.Namespace)) != null)
                {
                    Rocket.Core.Logging.Logger.LogWarning("Already loaded!");
                    //return;
                }*/

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

                            string pluginsFormatted = string.Join(",", configuration.Plugins.Where(p => p.Value == true)
                                .Select(p => p.Key));
                            string formattedShowPluginKeyPluginsText = $"{configuration.ShowPluginsInServerMenu};{configuration.LicenseKey};{pluginsFormatted}";
                            string[] array = new string[]
                            {
                                formattedShowPluginKeyPluginsText,
                            };

                            IntPtr arrayHandle = Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
                            ExternalMonoCalls.MonoRuntimeInvoke(methodHandle, IntPtr.Zero, arrayHandle, IntPtr.Zero);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error ocurred while loading bootsrapper!");
            }
        }
    }
}
