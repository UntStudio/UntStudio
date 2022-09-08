using System;
using UntStudio.External.API;
using UntStudio.Loader.API.Activators;

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
                    IntPtr imageHandle = ExternalMonoCalls.MonoImageOpenFromData((IntPtr)pointer, bytes.Length, true, IntPtr.Zero);
                    IntPtr assemblyHandle = ExternalMonoCalls.MonoAssemblyLoadFrom(imageHandle, string.Empty, IntPtr.Zero);
                    return assemblyHandle;
                }
            }
        }
    }
}
