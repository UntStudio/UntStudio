using System.Threading.Tasks;

namespace UntStudio.Loader.API.Decryptors
{
    public interface IDecryptor
    {
        Task<byte[]> DecryptAsync(byte[] bytes, string key);
    }
}