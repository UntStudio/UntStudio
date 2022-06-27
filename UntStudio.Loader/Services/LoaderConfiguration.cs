namespace UntStudio.Loader.Services
{
    internal sealed class LoaderConfiguration : ILoaderConfiguration
    {
        public string Key { get; }

        public string[] Plugins { get; }



        public LoaderConfiguration(string key, string[] plugins)
        {
            Key = key;
            Plugins = plugins;
        }
    }
}
