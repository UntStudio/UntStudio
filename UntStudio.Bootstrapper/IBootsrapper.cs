using System.Threading;
using System.Threading.Tasks;
using UntStudio.Bootstrapper.API;

namespace UntStudio.Bootstrapper
{
    internal interface IBootsrapper
    {
        Task<ServerResult> GetUnloadLoaderAsync(string key, CancellationToken cancellationToken = default);
    }
}