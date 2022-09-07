using System;
using System.Reflection;

namespace UntStudio.Loader.API;

public static class Events
{
    public static Action<IntPtr, Assembly, PluginFramework> OnLoadAssemblyRequested;
}
