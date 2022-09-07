using System.Threading;
using System.Threading.Tasks;
using UntStudio.Loader.API.Models;

namespace UntStudio.Loader.API;

public interface IServer
{
    Task<ServerResult> UploadPluginAsync(string licenseKey, string name, CancellationToken cancellationToken = default);
}