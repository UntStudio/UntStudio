namespace UntStudio.Loader.Services;

internal sealed class LoaderConfiguration : ILoaderConfiguration
{
    public bool ShowPlugins { get; }

    public string LicenseKey { get; }

    public string[] Plugins { get; }



    public LoaderConfiguration(bool showPlugins, string licenseKey, string[] plugins)
    {
        ShowPlugins = showPlugins;
        LicenseKey = licenseKey;
        Plugins = plugins;
    }
}
