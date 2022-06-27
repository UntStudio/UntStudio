using Rocket.Core.Plugins;
using SDG.Unturned;
using System.Runtime.InteropServices;
using UntStudio.Bootstrapper.API;
using UntStudio.Bootstrapper.Loaders;

namespace UntStudio.Bootstrapper
{
    internal sealed class Startup : RocketPlugin<BootsrapperConfiguration>
    {
        private static IBootsrapper bootsrapper;



        protected override async void Load()
        {
            bootsrapper = new Bootsrapper();
            ServerResult serverResult = await bootsrapper.GetUnloadLoaderAsync(Configuration.Instance.Key);
            if (serverResult.HasResponse)
            {
                Rocket.Core.Logging.Logger.Log("Response from server: " + serverResult.Response.Code);
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
                        IntPtr imageHandle = AssemblyLoader.MonoImageOpenFromData((IntPtr)pointer, serverResult.Bytes.Length, false, out int status1);
                        AssemblyLoader.MonoAssemblyLoadFrom(imageHandle, string.Empty, out int status2);
                        IntPtr classHandle = AssemblyLoader.MonoClassFromName(imageHandle, "UntStudio.Loader", "Loader");
                        IntPtr methodHandle = AssemblyLoader.MonoClassGetMethodFromName(classHandle, "Create", 1);

                        string pluginsFormatted = string.Join(",", Configuration.Instance.Plugins.Select(p => p));
                        string formattedKeyPluginsText = $"{Configuration.Instance.Key};{pluginsFormatted}";
                        IntPtr formattedKeyPluginsTextHandle = Marshal.StringToCoTaskMemUni(formattedKeyPluginsText);
                        AssemblyLoader.MonoRuntimeInvoke(methodHandle, IntPtr.Zero, formattedKeyPluginsTextHandle, IntPtr.Zero);
                    }
                }

                if (Configuration.Instance.DisplayLoaderInServerPluginsMenu)
                {
                    PluginAdvertising.Get().AddPlugin(typeof(Startup).Namespace);
                }

                if (Configuration.Instance.DisplayPluginsInServerPluginsMenu)
                {
                    PluginAdvertising.Get().AddPlugins(enabledPlugins);
                }
            }
        }
    }
}
