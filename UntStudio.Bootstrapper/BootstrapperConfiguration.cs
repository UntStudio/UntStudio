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

        public List<UntStudioPlugin> UntStudioPlugins;



        public void LoadDefaults()
        {
            Key = "XXXX-XXXX-XXXX-XXXX";
            DisplayLoaderInServerPluginsMenu = true;
            DisplayPluginsInServerPluginsMenu = true;
            UntStudioPlugins = new List<UntStudioPlugin>
            {
                new UntStudioPlugin(name: "UntStudio.OtherPlugin", enabled: true),
                new UntStudioPlugin(name: "UntStudio.PluginName", enabled: true),
            };
        }
    }
}
