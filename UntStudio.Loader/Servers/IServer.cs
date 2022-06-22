namespace UntStudio.Loader.Servers
{
    internal interface IServer
    {
        void SendRequest(string key, string pluginName);
    }
}