using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;
using System.Reflection;
using UntStudio.Loader.API;

[assembly: PluginMetadata("UntStudio.Loader.OpenMod.Integration")]
namespace UntStudio.Loader.OpenMod.Integration
{
    public sealed class Plugin : OpenModUnturnedPlugin
    {
        private readonly IPluginActivator pluginActivator;
        private readonly ILogger<Plugin> logger;

        public Plugin(IServiceProvider serviceProvider, IPluginActivator pluginActivator, ILogger<Plugin> logger) : base(serviceProvider)
        {
            this.pluginActivator = pluginActivator;
            this.logger = logger;
        }


        protected override UniTask OnLoadAsync()
        {
            Events.OnLoadAssemblyRequested += onLoadAssemblyRequestedAsync;
            return UniTask.CompletedTask;
        }

        protected override UniTask OnUnloadAsync()
        {
            Events.OnLoadAssemblyRequested -= onLoadAssemblyRequestedAsync;
            return UniTask.CompletedTask;
        }


        private async void onLoadAssemblyRequestedAsync(IntPtr assemblyHandle, Assembly assembly, PluginFramework pluginFramework)
        {
            if (pluginFramework.Equals(PluginFramework.OpenMod))
            {
                IOpenModPlugin openModPlugin = await this.pluginActivator.TryActivatePluginAsync(assembly);
                if (openModPlugin == null)
                {
                    this.logger.LogWarning($"OpenMod Integration didnt work out to load plugin {assembly.GetName().Name}!");
                }
                else
                {
                    this.logger.LogInformation($"OpenMod Integration Loading new plugin: {openModPlugin.DisplayName}!");
                }
            }
        }
    }
}
