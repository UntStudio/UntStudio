using System;
using System.Runtime.InteropServices;

namespace UntStudio.Bootstrapper.Loaders
{
    internal sealed class ExternalMonoCalls
    {
        [DllImport("__Internal", EntryPoint = "mono_image_open_from_data")]
        internal static extern IntPtr MonoImageOpenFromData(IntPtr dataHandle, int dataLenght, bool shouldCopy, out int status);

        [DllImport("__Internal", EntryPoint = "mono_assembly_load_from")]
        internal static extern IntPtr MonoAssemblyLoadFrom(IntPtr imageHandle, string name, out int status);

        [DllImport("__Internal", EntryPoint = "mono_class_from_name")]
        internal static extern IntPtr MonoClassFromName(IntPtr assemblyHandle, string @namespace, string @class);

        [DllImport("__Internal", EntryPoint = "mono_class_get_method_from_name")]
        internal static extern IntPtr MonoClassGetMethodFromName(IntPtr classHandle, string methodName, int parametersCount);

        [DllImport("__Internal", EntryPoint = "mono_string_new")]
        internal static extern IntPtr MonoStringNew(IntPtr domainHandle, string text);

        [DllImport("__Internal", EntryPoint = "mono_domain_get")]
        internal static extern IntPtr MonoDomainGet();

        [DllImport("__Internal", EntryPoint = "mono_init_version")]
        internal static extern IntPtr MonoInitVersion(string domainName, string version);

        [DllImport("__Internal", EntryPoint = "mono_runtime_invoke")]
        internal static extern IntPtr MonoRuntimeInvoke(IntPtr methodHandle, IntPtr instanceHandle, IntPtr parametersHandle, IntPtr exceptionInformationHandle);
    }
}
