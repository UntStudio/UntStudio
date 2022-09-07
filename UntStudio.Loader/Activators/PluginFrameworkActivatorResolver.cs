using System.Linq;
using System.Reflection;

namespace UntStudio.Loader.Activators
{
    public sealed class PluginFrameworkActivatorResolver : IPluginFrameworkActivatorResolver
    {
        private readonly IRocketModPluginActivator rocketModPluginActivator;
        private readonly IOpenModPluginActivator openModPluginActivator;

        public PluginFrameworkActivatorResolver(IRocketModPluginActivator rocketModPluginActivator, IOpenModPluginActivator openModPluginActivator)
        {
            this.rocketModPluginActivator = rocketModPluginActivator;
            this.openModPluginActivator = openModPluginActivator;
        }


        public IPluginActivator Resolve(Assembly assembly)
        {
            if (assembly.GetTypes().FirstOrDefault(t => t.GetInterface(KnownInitialPluginFrameworkTypes.RocketModInterface) != null) != null)
            {
                return this.rocketModPluginActivator;
            }
            else if (assembly.GetTypes().FirstOrDefault(t => t.GetInterface(KnownInitialPluginFrameworkTypes.OpenModInterface) != null) != null)
            {
                return this.openModPluginActivator;
            }
            else
            {
                throw new UnsupportedPluginFrameworkException();
            }
        }
    }
}
