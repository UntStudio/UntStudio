using System.Threading.Tasks;
using UntStudio.API.Bootstrapper.Models;

namespace UntStudio.Bootstrapper.API
{
    public interface IBootstrapper
    {
        Task<ServerResult> UploadLoaderAsync(string key);

        Task<ServerResult> GetLoaderEntryPointAsync(string key);
    }
}