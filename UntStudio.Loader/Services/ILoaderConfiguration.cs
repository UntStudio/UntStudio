namespace UntStudio.Loader.Services
{
    internal interface ILoaderConfiguration
    {
        string Key { get; }

        string[] Plugins { get; }
    }
}