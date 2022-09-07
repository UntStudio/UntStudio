using System.Threading.Tasks;

namespace UntStudio.Loader.Decryptors
{
    public interface IDecryptor
    {
        Task<byte[]> DecryptAsync(byte[] bytes, string key);
    }
}