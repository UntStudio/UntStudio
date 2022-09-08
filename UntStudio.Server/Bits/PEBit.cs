using System.IO;

namespace UntStudio.Server.Bits
{
    public sealed class PEBit : IPEBit
    {
        public byte[] Bit(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            using (var reader = new BinaryReader(stream))
            using (var writer = new BinaryWriter(stream))
            {
                int peLenght = 33;
                for (int i = 0; i < peLenght; i++)
                {
                    writer.Write(0); // clears all dos + pe
                }

                var numberOfRVaAndSizes = 0xF4;
                stream.Position = numberOfRVaAndSizes;
                writer.Write(0xD); // makes fool

                var dotnetSize = 0x16C;
                stream.Position = dotnetSize;
                writer.Write(0); // breaks ILspy (but only for decompilers, Unturned game server is working great)

                var debugVirtualAddress = 0x128;
                stream.Position = debugVirtualAddress;
                writer.Write(0); // didnt affect to mono

                var debugSize = 0x12C;
                stream.Position = debugSize;
                writer.Write(0); //didnt affect to mono

                var importSize = 0x104;
                stream.Position = importSize;
                writer.Write(0); // didnt affect to mono, but may to some decompilers

                var baseRelocSize = 0x124;
                stream.Position = baseRelocSize;
                writer.Write(0); // breaks mono

                return stream.ToArray();
            }
        }
    }
}
