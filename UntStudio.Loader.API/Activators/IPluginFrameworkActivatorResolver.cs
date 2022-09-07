using System.Reflection;

namespace UntStudio.Loader.API.Activators;

public interface IPluginFrameworkActivatorResolver
{
    IPluginActivator Resolve(Assembly assembly);
}
