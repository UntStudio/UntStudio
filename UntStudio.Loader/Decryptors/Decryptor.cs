using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UntStudio.Loader.Decryptors
{
    internal sealed class Decryptor : IDecryptor
    {
        private byte[] saltBytes = new byte[]
        {
            0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8,
            0x1, 0x02, 0x3, 0x4, 0x5, 0x6, 0x7, 0x1,
            0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8,
            0x1, 0x02, 0x3, 0x4, 0x5, 0x6, 0x7, 0x1,
            0x1, 0x01, 0x2, 0x3, 0x50, 0x61, 0x79, 0x10,
        };



		public async Task<string> DecryptAsync(byte[] bytes, string key)
		{
			byte[] decryptedBytes = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (RijndaelManaged aes = new RijndaelManaged())
				{
					aes.KeySize = 256;
					aes.BlockSize = 128;
					Rfc2898DeriveBytes keyRfc = new Rfc2898DeriveBytes(key, saltBytes, 1000);
					aes.Key = keyRfc.GetBytes(aes.KeySize / 8);
					aes.IV = keyRfc.GetBytes(aes.BlockSize / 8);
					aes.Mode = CipherMode.CBC;

					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
					{
						await cryptoStream.WriteAsync(bytes, 0, bytes.Length);
						cryptoStream.Close();
					}

					decryptedBytes = memoryStream.ToArray();
				}
			}

			return Encoding.UTF8.GetString(decryptedBytes);
		}
	}
}
