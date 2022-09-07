using System;
using System.Reflection;
using UntStudio.Loader.API;
using UntStudio.Loader.API.Activators;

namespace UntStudio.Loader.Activators
{
    public sealed class OpenModPluginActivator : IOpenModPluginActivator
    {
        public void Activate(IntPtr handle, Assembly assembly)
        {
            Events.OnLoadAssemblyRequested?.Invoke(handle, assembly, PluginFramework.OpenMod);
        }
    }
}
