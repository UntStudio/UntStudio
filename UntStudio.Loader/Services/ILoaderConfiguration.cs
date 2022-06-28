namespace UntStudio.Loader.Services
{
    public interface ILoaderConfiguration
    {
        string Key { get; }

        string[] Plugins { get; }
    }
}