using System.Threading.Tasks;

namespace UntStudio.Server.Encryptors
{
    public interface IEncryptor
    {
        Task<byte[]> EncryptContentAsync(string text, string key);
    }
}