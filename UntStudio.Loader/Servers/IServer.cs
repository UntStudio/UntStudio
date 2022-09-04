using System.Threading;
using System.Threading.Tasks;
using UntStudio.Loader.Models;

namespace UntStudio.Loader.Servers;

public interface IServer
{
    Task<ServerResult> UploadPluginAsync(string licenseKey, string name, CancellationToken cancellationToken = default);
}