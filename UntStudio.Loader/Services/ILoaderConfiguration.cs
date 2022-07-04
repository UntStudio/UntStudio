namespace UntStudio.Loader.Services
{
    public interface ILoaderConfiguration
    {
        bool ShowPlugins { get; }

        string Key { get; }

        string[] Plugins { get; }
    }
}