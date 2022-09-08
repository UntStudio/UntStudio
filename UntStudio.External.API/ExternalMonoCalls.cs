using System;
using System.Runtime.InteropServices;

namespace UntStudio.External.API;

public static class ExternalMonoCalls
{
    [DllImport("__Internal", EntryPoint = "mono_image_open_from_data")]
    public static extern IntPtr MonoImageOpenFromData(IntPtr dataHandle, int dataLenght, bool shouldCopy, IntPtr status);

    [DllImport("__Internal", EntryPoint = "mono_assembly_load_from")]
    public static extern IntPtr MonoAssemblyLoadFrom(IntPtr imageHandle, string name, IntPtr status);

    [DllImport("__Internal", EntryPoint = "mono_class_from_name")]
    public static extern IntPtr MonoClassFromName(IntPtr assemblyHandle, string @namespace, string @class);

    [DllImport("__Internal", EntryPoint = "mono_class_get_method_from_name")]
    public static extern IntPtr MonoClassGetMethodFromName(IntPtr classHandle, string methodName, int parametersCount);

    [DllImport("__Internal", EntryPoint = "mono_runtime_invoke")]
    public static extern IntPtr MonoRuntimeInvoke(IntPtr methodHandle, IntPtr instanceHandle, IntPtr parametersHandle, IntPtr exceptionInformationHandle);
}
