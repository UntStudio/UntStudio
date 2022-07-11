using System.Threading.Tasks;
using UntStudio.Bootstrapper.API;

namespace UntStudio.Bootstrapper
{
    internal interface IBootstrapper
    {
        Task<ServerResult> UploadLoaderAsync(string key);

        Task<ServerResult> GetLoaderEntryPointAsync(string key);
    }
}