using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.Linq;
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
                Rocket.Core.Logging.Logger.Log("###FLAG 0");
                IBootstrapper bootstrapper = new Bootstrapper();
                Rocket.Core.Logging.Logger.Log("###FLAG 1");

                ServerResult loaderServerResult = await bootstrapper.GetUnloadLoaderAsync(Configuration.Instance.Key);
                Rocket.Core.Logging.Logger.Log("###FLAG 2");

                if (loaderServerResult == null)
                {
                    Rocket.Core.Logging.Logger.Log("###FLAG 2.0");
                    Rocket.Core.Logging.Logger.Log("Cannot load loader!");
                    return;
                }

                if (loaderServerResult.HasResponse)
                {
                    Rocket.Core.Logging.Logger.Log("###FLAG 3");

                    Rocket.Core.Logging.Logger.LogWarning("You have a new response from server!");
                    Rocket.Core.Logging.Logger.Log("###FLAG 4");

                    Rocket.Core.Logging.Logger.LogWarning(translateServerResponse(loaderServerResult.Response.Code));
                    Rocket.Core.Logging.Logger.Log("###FLAG 4.1");
                    return;
                }


                Rocket.Core.Logging.Logger.Log("###FLAG 5");

                ServerResult loaderEntryPointServerResult = await bootstrapper.GetLoaderEntryPointAsync(Configuration.Instance.Key);
                Rocket.Core.Logging.Logger.Log("###FLAG 6");

                if (loaderEntryPointServerResult == null)
                {
                    Rocket.Core.Logging.Logger.Log("Cannot load loader!");
                    return;
                }

                if (loaderEntryPointServerResult.HasResponse)
                {
                    Rocket.Core.Logging.Logger.Log("###FLAG 7");

                    Rocket.Core.Logging.Logger.LogWarning("You have a new response from server!");
                    Rocket.Core.Logging.Logger.Log("###FLAG 8");
                    Rocket.Core.Logging.Logger.Log("###FLAG 8.0: Message: " + loaderEntryPointServerResult.Response.Code.ToString());

                    Rocket.Core.Logging.Logger.LogWarning(translateServerResponse(loaderEntryPointServerResult.Response.Code));
                    Rocket.Core.Logging.Logger.Log("###FLAG 9");

                    return;
                }

                if (loaderEntryPointServerResult.HasLoaderEntryPoint == false)
                {
                    Rocket.Core.Logging.Logger.Log("###FLAG 9.00");
                    Rocket.Core.Logging.Logger.Log("Cannot load loader!");

                    return;
                }

                Rocket.Core.Logging.Logger.Log("###FLAG 10");

                if (loaderServerResult.HasBytes)
                {
                    Rocket.Core.Logging.Logger.Log("###FLAG 11");

                    foreach (Plugin plugin in Configuration.Instance.UntStudioPlugins)
                    {
                        if (plugin.Enabled)
                        {
                            await bootstrapper.PutUnblockPluginAsync(Configuration.Instance.Key, plugin.Name);
                        }
                        else
                        {
                            await bootstrapper.PutBlockPluginAsync(Configuration.Instance.Key, plugin.Name);
                        }
                    }

                    string[] enabledPlugins = Configuration.Instance.UntStudioPlugins
                        .Where(p => p.Enabled)
                        .Select(p => p.Name)
                        .ToArray();

                    Rocket.Core.Logging.Logger.Log("###ENABLED PLUGINS:");
                    for (int i = 0; i < enabledPlugins.Length; i++)
                    {
                        Rocket.Core.Logging.Logger.Log(enabledPlugins[i]);
                    }
                    Rocket.Core.Logging.Logger.Log("###END OF ENABLED PLUGINS:");


                    Rocket.Core.Logging.Logger.Log("###FLAG 12");

                    unsafe
                    {
                        Rocket.Core.Logging.Logger.Log("###FLAG 13");

                        fixed (byte* pointer = loaderServerResult.Bytes)
                        {
                            Rocket.Core.Logging.Logger.Log("###FLAG 14");

                            IntPtr imageHandle = ExternalMonoCalls.MonoImageOpenFromData((IntPtr)pointer, loaderServerResult.Bytes.Length, false, out _);
                            Rocket.Core.Logging.Logger.Log("###FLAG 15");

                            ExternalMonoCalls.MonoAssemblyLoadFrom(imageHandle, string.Empty, out _);
                            Rocket.Core.Logging.Logger.Log("###FLAG 16");

                            IntPtr classHandle = ExternalMonoCalls.MonoClassFromName(imageHandle,
                                loaderEntryPointServerResult.LoaderEntryPoint.Namespace,
                                loaderEntryPointServerResult.LoaderEntryPoint.Class);
                            Rocket.Core.Logging.Logger.Log("###FLAG 17");

                            /*IntPtr methodHandle = ExternalMonoCalls.MonoClassGetMethodFromName(classHandle,
                                loaderEntryPointServerResult.LoaderEntryPoint.Method, 1);*/

                            IntPtr methodHandle = ExternalMonoCalls.MonoClassGetMethodFromName(classHandle,
                                loaderEntryPointServerResult.LoaderEntryPoint.Method, 0);

                            Rocket.Core.Logging.Logger.Log("###FLAG 18");

                            string pluginsFormatted = string.Join(",", enabledPlugins.Select(p => p));
                            Rocket.Core.Logging.Logger.Log("###FLAG 19");

                            string formattedKeyPluginsText = $"{Configuration.Instance.Key};{pluginsFormatted}";
                            Rocket.Core.Logging.Logger.Log("###FLAG 20");

                            //IntPtr formattedKeyPluginsTextHandle = Marshal.StringToCoTaskMemUni(formattedKeyPluginsText);
                            //IntPtr formattedKeyPluginsTextHandle = new IntPtr(Convert.ToInt32(formattedKeyPluginsText));
                            //IntPtr formattedKeyPluginsTextHandle = Marshal.PtrToStringAuto(formattedKeyPluginsText);
                            //formattedKeyPluginsTextHandle = Marshal.StringToHGlobalUni(formattedKeyPluginsText);

                            Rocket.Core.Logging.Logger.Log("###FLAG 21");

                            //ExternalMonoCalls.MonoRuntimeInvoke(methodHandle, IntPtr.Zero, formattedKeyPluginsTextHandle, IntPtr.Zero);
                            ExternalMonoCalls.MonoRuntimeInvoke(methodHandle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                            Rocket.Core.Logging.Logger.Log("###FLAG 22");
                        }
                        Rocket.Core.Logging.Logger.Log("###FLAG 23");
                    }

                    Rocket.Core.Logging.Logger.Log("###FLAG 24");

                    if (Configuration.Instance.DisplayLoaderInServerPluginsMenu)
                    {
                        Rocket.Core.Logging.Logger.Log("###FLAG 25");

                        PluginAdvertising.Get().AddPlugin(typeof(Startup).Namespace);
                        Rocket.Core.Logging.Logger.Log("###FLAG 26");

                    }

                    Rocket.Core.Logging.Logger.Log("###FLAG 27");

                    if (Configuration.Instance.DisplayLoaderInServerPluginsMenu)
                    {
                        Rocket.Core.Logging.Logger.Log("###FLAG 28");

                        PluginAdvertising.Get().AddPlugins(enabledPlugins);
                        Rocket.Core.Logging.Logger.Log("###FLAG 29");

                    }
                    Rocket.Core.Logging.Logger.Log("###FLAG 31");

                }
                Rocket.Core.Logging.Logger.Log("###FLAG 32");
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "An error ocurred while loading bootsrapper!");
            }
            Rocket.Core.Logging.Logger.Log("###FLAG 33");
        }



        private string translateServerResponse(CodeResponse code)
        {
            Rocket.Core.Logging.Logger.Log("###FLAG 100");

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
