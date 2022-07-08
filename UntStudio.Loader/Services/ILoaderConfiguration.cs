namespace UntStudio.Loader.Services;

public interface ILoaderConfiguration
{
    bool ShowPlugins { get; }

    string LicenseKey { get; }

    string[] Plugins { get; }
}