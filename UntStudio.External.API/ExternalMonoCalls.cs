using System;
using System.Runtime.InteropServices;

namespace UntStudio.External.API;

public static class ExternalMonoCalls
{
    [DllImport("__Internal", EntryPoint = "mono_image_open_from_data")]
    public static extern IntPtr MonoImageOpenFromData(IntPtr dataHandle, int dataLenght, bool shouldCopy, IntPtr status);

    [DllImport("__Internal", EntryPoint = "mono_assembly_load_from")]
    public static extern IntPtr MonoAssemblyLoadFrom(IntPtr imageHandle, string name, IntPtr status);

    [DllImport("__Internal", EntryPoint = "mono_assembly_load_from_full")]
    public static extern IntPtr MonoAssemblyLoadFromFull(IntPtr imageHandle, string name, IntPtr monoImageOpenStatus, bool reflectionOnly);

    [DllImport("__Internal", EntryPoint = "mono_class_from_name")]
    public static extern IntPtr MonoClassFromName(IntPtr assemblyHandle, string @namespace, string @class);

    [DllImport("__Internal", EntryPoint = "mono_class_get_method_from_name")]
    public static extern IntPtr MonoClassGetMethodFromName(IntPtr classHandle, string methodName, int parametersCount);

    [DllImport("__Internal", EntryPoint = "mono_runtime_invoke")]
    public static extern IntPtr MonoRuntimeInvoke(IntPtr methodHandle, IntPtr instanceHandle, IntPtr parametersHandle, IntPtr exceptionInformationHandle);




    [DllImport("__Internal", EntryPoint = "mono_assembly_close")]
    public static extern void MonoAssemblyClose(IntPtr assemblyHandle);

    [DllImport("__Internal", EntryPoint = "mono_assemblies_cleanup")]
    public static extern void MonoModuleAssembliesCleanup();

    [DllImport("__Internal", EntryPoint = "mono_threads_request_thread_dump")]
    public static extern void MonoThreadsRequestThreadDump();

    [DllImport("__Internal", EntryPoint = "mono_gc_max_generation")]
    public static extern int MonoGarbageCollectorMaxGeneration();

    [DllImport("__Internal", EntryPoint = "mono_gc_collection_count ")]
    public static extern void MonoGarbageCollectorCollectionCount(int generation);
}
