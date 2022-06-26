using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using XSystem.Security.Cryptography;

namespace UntStudio.Server.Repositories;

public sealed class LoaderHashesVerifierRepository : IHashesVerifierRepository
{
    private readonly IConfiguration configuration;



    public LoaderHashesVerifierRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }



    public bool Verify(byte[] bytes)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        string realLoaderHash = GetHashFrom(this.configuration["PluginsLoader:Path"]);
        string tempBytesFilePathHolder = Path.Combine(this.configuration["Temp:Path"], bytes.GetLongLength(0).ToString(), ".dll");
        File.WriteAllBytes
        (
            path: tempBytesFilePathHolder, 
            bytes: bytes
        );
        string tempBytesHash = GetHashFrom(tempBytesFilePathHolder);
        return realLoaderHash.Equals(tempBytesHash);
    }

    public string GetHashFrom(string file)
    {
        string hash = null;
        using (FileStream fileStream = File.OpenRead(file))
        {
            SHA256Managed sha256 = new SHA256Managed();
            byte[] hashBytes = sha256.ComputeHash(fileStream);

            hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
        return hash;
    }
}
