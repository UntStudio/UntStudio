namespace UntStudio.Loader.Servers
{
    internal interface IServer
    {
        Task<ServerResult> GetUnloadPluginAsync(string key, string name, CancellationToken cancellationToken = default);
    }
}