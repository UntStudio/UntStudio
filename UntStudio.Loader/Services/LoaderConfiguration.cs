namespace UntStudio.Loader.Services
{
    internal sealed class LoaderConfiguration : ILoaderConfiguration
    {
        public bool ShowPlugins { get; }

        public string Key { get; }

        public string[] Plugins { get; }



        public LoaderConfiguration(bool showPlugins, string key, string[] plugins)
        {
            ShowPlugins = showPlugins;
            Key = key;
            Plugins = plugins;
        }
    }
}
