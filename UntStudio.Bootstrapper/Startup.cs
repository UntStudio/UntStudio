using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UntStudio.Bootstrapper.API;
using UntStudio.Bootstrapper.Loaders;
using static UntStudio.Bootstrapper.API.RequestResponse;

namespace UntStudio.Bootstrapper
{
    internal sealed class Startup : RocketPlugin<BootsrapperConfiguration>
    {
        protected override async void Load()
        {
            ServerResult serverResult = await new Bootsrapper().GetUnloadLoaderAsync(Configuration.Instance.Key);

            if (serverResult.HasResponse)
            {
                Rocket.Core.Logging.Logger.LogWarning("You have a new response from server!");
                string message = serverResult.Response.Code switch
                {
                    CodeResponse.VersionOutdated                         => "Loader version outdated, please download latest!",
                    CodeResponse.KeyValidationFailed                     => "Please, check your key, and write it properly!",
                    CodeResponse.NameValidationFailed                    => "Plugin name validation failed, please verify your plugin configuration.",
                    CodeResponse.IPNotBindedOrSpecifiedKeyOrNameNotFound => "Your key is not binded or key does not exist or plugin name not found.",
                    CodeResponse.SubscriptionBanned                      => "Your subscription banned.",
                    CodeResponse.SubscriptionExpired                     => "Your subscription expired.",
                    _ => "Unknown server response, please contact with Administrator.",
                };
                Rocket.Core.Logging.Logger.LogWarning(message);
            }
            
            if (serverResult.HasBytes)
            {
                string[] enabledPlugins = Configuration.Instance.Plugins
                    .Where(p => p.Enabled)
                    .Select(p => p.Name)
                    .ToArray();

                unsafe
                {
                    fixed (byte* pointer = serverResult.Bytes)
                    {
                        IntPtr imageHandle = ExternalMonoCalls.MonoImageOpenFromData((IntPtr)pointer, serverResult.Bytes.Length, false, out _);
                        ExternalMonoCalls.MonoAssemblyLoadFrom(imageHandle, string.Empty, out _);
                        IntPtr classHandle = ExternalMonoCalls.MonoClassFromName(imageHandle, "UntStudio.Loader", "Loader");
                        IntPtr methodHandle = ExternalMonoCalls.MonoClassGetMethodFromName(classHandle, "Create", 1);

                        string pluginsFormatted = string.Join(",", enabledPlugins.Select(p => p));
                        string formattedKeyPluginsText = $"{Configuration.Instance.Key};{pluginsFormatted}";
                        IntPtr formattedKeyPluginsTextHandle = Marshal.StringToCoTaskMemUni(formattedKeyPluginsText);
                        ExternalMonoCalls.MonoRuntimeInvoke(methodHandle, IntPtr.Zero, formattedKeyPluginsTextHandle, IntPtr.Zero);
                    }
                }

                if (Configuration.Instance.DisplayLoaderInServerPluginsMenu)
                {
                    PluginAdvertising.Get().AddPlugin(typeof(Startup).Namespace);
                }

                if (Configuration.Instance.DisplayLoaderInServerPluginsMenu)
                {
                    PluginAdvertising.Get().AddPlugins(enabledPlugins);
                }
            }
        }
    }
}
