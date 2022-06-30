using System.Threading;
using System.Threading.Tasks;
using UntStudio.Loader.API;

namespace UntStudio.Loader.Servers
{
    public interface IServer
    {
        Task<ServerResult> GetUnloadPluginAsync(string key, string name, CancellationToken cancellationToken = default);
    }
}