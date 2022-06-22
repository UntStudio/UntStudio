using System.Reflection;

namespace UntStudio.Loader.Servers
{
    internal sealed class Server : IServer
    {
        public void SendRequest(string key, string pluginName)
        {
            byte[] bytes = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
        }
    }
}
