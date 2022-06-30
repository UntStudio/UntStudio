using Rocket.API;
using System.Collections.Generic;
using UntStudio.Bootstrapper.Models;

namespace UntStudio.Bootstrapper
{
    public sealed class BootstrapperConfiguration : IRocketPluginConfiguration
    {
        public string Key;

        public bool DisplayLoaderInServerPluginsMenu;

        public bool DisplayPluginsInServerPluginsMenu;

        public List<Plugin> UntStudioPlugins;



        public void LoadDefaults()
        {
            Key = "XXXX-XXXX-XXXX-XXXX";
            DisplayLoaderInServerPluginsMenu = true;
            DisplayPluginsInServerPluginsMenu = true;
            UntStudioPlugins = new List<Plugin>
            {
                new Plugin(name: "UntStudio.RustSleeper", enabled: true),
                new Plugin(name: "UntStudio.PluginName", enabled: true),
            };
        }
    }
}
