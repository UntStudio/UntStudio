using System.Threading.Tasks;
using UntStudio.Bootstrapper.Models;

namespace UntStudio.Bootstrapper
{
    internal interface IBootstrapper
    {
        Task<ServerResult> UploadLoaderAsync(string key);

        Task<ServerResult> GetLoaderEntryPointAsync(string key);
    }
}