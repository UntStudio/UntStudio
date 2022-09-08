using System.Collections.Generic;

namespace UntStudio.API.Bootstrapper.Models
{
    public class Configuration
    {
        public string LicenseKey;

        public bool ShowPluginsInServerMenu;

        public Dictionary<string, bool> Plugins;



        public Configuration(string licenseKey, bool showPluginsInServerMenu, Dictionary<string, bool> plugins)
        {
            LicenseKey = licenseKey;
            ShowPluginsInServerMenu = showPluginsInServerMenu;
            Plugins = plugins;
        }
    }
}
