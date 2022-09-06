using System;
using UntStudio.Loader.External;

namespace UntStudio.Loader.Activators
{
    public sealed class MonoActivator : IMonoActivator
    {
        public IntPtr Activate(byte[] bytes)
        {
            unsafe
            {
                fixed (byte* pointer = bytes)
                {
                    IntPtr imageHandle = ExternalMonoCalls.MonoImageOpenFromData((IntPtr)pointer, bytes.Length, false, out _);
                    IntPtr assemblyHandle = ExternalMonoCalls.MonoAssemblyLoadFrom(imageHandle, string.Empty, out _);
                    return assemblyHandle;
                }
            }
        }
    }
}
