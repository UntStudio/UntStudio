using System;
using System.Runtime.InteropServices;

namespace UntStudio.Loader.API
{
    public static class ExternalMonoAPI
    {
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
}
