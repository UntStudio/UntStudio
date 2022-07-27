using System.Threading.Tasks;

namespace UntStudio.Loader.Decryptors
{
    public interface IDecryptor
    {
        Task<string> DecryptAsync(byte[] bytes, string key);
    }
}