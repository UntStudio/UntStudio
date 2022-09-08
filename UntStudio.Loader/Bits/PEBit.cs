using System.IO;
using UntStudio.Loader.API.PortableExecutable;

namespace UntStudio.Loader.Solvers
{
    internal sealed class PEBit : IPEBit
    {
        private readonly ulong[] normalPE = new ulong[]
        {
            0x300905A4D,
            0xFFFF00000004,
            0xB8,
            0x40,
            0x0,
            0x0,
            0x0,
            0x8000000000,
            0xCD09B4000EBA1F0E,
            0x685421CD4C01B821,
            0x72676F7270207369,
            0x6F6E6E6163206D61,
            0x6E75722065622074,
            0x20534F44206E6920,
            0xA0D0D2E65646F6D,
            0x24,
            0x3014C00004550
        };

        public byte[] Unbit(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            using (var reader = new BinaryReader(stream))
            using (var writer = new BinaryWriter(stream))
            {
                for (int i = 0; i < normalPE.Length; i++)
                {
                    writer.Write(normalPE[i]);
                }

                var numberOfRVaAndSizes = 0xF4;
                stream.Position = numberOfRVaAndSizes;
                writer.Write(0xA);

                var baseRelocSize = 0x124;
                stream.Position = baseRelocSize;
                writer.Write(0xC);

                return stream.ToArray();
            }
        }
    }
}
