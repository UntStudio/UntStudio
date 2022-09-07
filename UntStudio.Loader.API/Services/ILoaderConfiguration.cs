namespace UntStudio.Loader.API.Services;

public interface ILoaderConfiguration
{
    bool ShowPlugins { get; }

    string LicenseKey { get; }

    string[] Plugins { get; }
}