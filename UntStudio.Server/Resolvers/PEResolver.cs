using System;
using System.IO;

namespace UntStudio.Server.Resolvers
{
    public sealed class PEResolver : IPEResolver
    {
        public byte[] Resolve(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            using (BinaryReader reader = new BinaryReader(stream))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                int peLenght = 33;
                for (int i = 0; i < peLenght; i++)
                {
                    writer.Write(0x0);
                }

                var numberOfRVaAndSizes = 0xF4;
                stream.Position = numberOfRVaAndSizes;
                writer.Write(0xD);

                return stream.ToArray();
            }
            return Array.Empty<byte>();
        }
    }
}
