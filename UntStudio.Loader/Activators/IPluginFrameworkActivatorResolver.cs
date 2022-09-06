using System.Reflection;

namespace UntStudio.Loader.Activators
{
    public interface IPluginFrameworkActivatorResolver
    {
        IPluginActivator Resolve(Assembly assembly);
    }
}
