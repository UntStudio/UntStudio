using System;
using System.Runtime.InteropServices;

namespace UntStudio.Bootstrapper.External
{
    internal static class ExternalMonoCalls
    {
        [DllImport("__Internal", EntryPoint = "mono_image_open_from_data")]
        internal static extern IntPtr MonoImageOpenFromData(IntPtr dataHandle, int dataLenght, bool shouldCopy, out int status);

        [DllImport("__Internal", EntryPoint = "mono_assembly_load_from")]
        internal static extern IntPtr MonoAssemblyLoadFrom(IntPtr imageHandle, string name, out int status);

        [DllImport("__Internal", EntryPoint = "mono_class_from_name")]
        internal static extern IntPtr MonoClassFromName(IntPtr assemblyHandle, string @namespace, string @class);

        [DllImport("__Internal", EntryPoint = "mono_class_get_method_from_name")]
        internal static extern IntPtr MonoClassGetMethodFromName(IntPtr classHandle, string methodName, int parametersCount);

        [DllImport("__Internal", EntryPoint = "mono_runtime_invoke")]
        internal static extern IntPtr MonoRuntimeInvoke(IntPtr methodHandle, IntPtr instanceHandle, IntPtr parametersHandle, IntPtr exceptionInformationHandle);
    }
}
