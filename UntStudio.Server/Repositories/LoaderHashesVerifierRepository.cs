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

        string realLoaderHash = GetHashFrom(File.ReadAllBytes(this.configuration["PluginsLoader:Path"]));
        string tempBytesHash = GetHashFrom(bytes);
        return realLoaderHash.Equals(tempBytesHash);
    }

    public string GetHashFrom(byte[] bytes)
    {
        string hash = null;
        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            SHA256Managed sha256 = new SHA256Managed();
            byte[] hashBytes = sha256.ComputeHash(memoryStream);

            hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
        return hash;
    }
}
