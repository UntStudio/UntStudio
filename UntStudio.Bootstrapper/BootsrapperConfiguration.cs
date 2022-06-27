using Rocket.API;
using UntStudio.Bootstrapper.Models;

namespace UntStudio.Bootstrapper
{
    internal sealed class BootsrapperConfiguration : IRocketPluginConfiguration
    {
        public string Key;

        public bool DisplayLoaderInServerPluginsMenu;

        public bool DisplayPluginsInServerPluginsMenu;

        public List<Plugin> Plugins;



        public void LoadDefaults()
        {
            Key = "XXXX-XXXX-XXXX-XXXX";
            DisplayLoaderInServerPluginsMenu = true;
            DisplayPluginsInServerPluginsMenu = true;
            Plugins = new List<Plugin>
            {
                new Plugin(name: "UntStudio.RustSleeper", enabled: true),
            };
        }
    }
}
