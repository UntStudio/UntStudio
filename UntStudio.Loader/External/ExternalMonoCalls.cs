using System;
using System.Runtime.InteropServices;

namespace UntStudio.Loader.External
{
    internal sealed class ExternalMonoCalls
    {
        [DllImport("__Internal", EntryPoint = "mono_image_open_from_data")]
        internal static extern IntPtr MonoImageOpenFromData(IntPtr dataHandle, int dataLenght, bool shouldCopy, out int status);

        [DllImport("__Internal", EntryPoint = "mono_assembly_load_from")]
        internal static extern IntPtr MonoAssemblyLoadFrom(IntPtr imageHandle, string name, out int status);
    }
}
