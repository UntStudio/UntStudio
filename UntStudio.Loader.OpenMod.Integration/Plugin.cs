using Cysharp.Threading.Tasks;
using dnlib.DotNet;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UntStudio.Loader.API;

[assembly: PluginMetadata("UntStudio.Loader.OpenMod.Integration")]
namespace UntStudio.Loader.OpenMod.Integration
{
    public sealed class Plugin : OpenModUnturnedPlugin
    {
        private readonly IPluginActivator pluginActivator;
        private readonly ILogger<Plugin> logger;
        private readonly HashSet<IntPtr> assemblyHandles;

        public Plugin(IServiceProvider serviceProvider, IPluginActivator pluginActivator, ILogger<Plugin> logger) : base(serviceProvider)
        {
            this.pluginActivator = pluginActivator;
            this.logger = logger;
            assemblyHandles = new HashSet<IntPtr>();
        }


        protected override UniTask OnLoadAsync()
        {
            Events.OnLoadAssemblyRequested += onLoadAssemblyRequested;
            return UniTask.CompletedTask;
        }

        protected override UniTask OnUnloadAsync()
        {
            Events.OnLoadAssemblyRequested -= onLoadAssemblyRequested;

            this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #1]->>>>>>>>>>>>>");
            //ExternalMonoAPI.MonoThreadsRequestThreadDump();
            this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #2]->>>>>>>>>>>>>");
            Thread.Sleep(2000);
            this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #3]->>>>>>>>>>>>>");
            //ExternalMonoAPI.MonoModuleAssembliesCleanup();
            this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #4]->>>>>>>>>>>>>");
            Thread.Sleep(2000);
            this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #5]->>>>>>>>>>>>>");
            //ExternalMonoAPI.MonoGarbageCollectorCollectionCount(ExternalMonoAPI.MonoGarbageCollectorMaxGeneration());
            this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #6]->>>>>>>>>>>>>");
            Thread.Sleep(2000);
            this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #7]->>>>>>>>>>>>>");
            foreach (var assemblyHandle in assemblyHandles)
            {
                this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #8]->>>>>>>>>>>>>");
                this.logger.LogInformation("Releasing OpenMod plugin!");
                this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #9]->>>>>>>>>>>>>");
                //ExternalMonoAPI.MonoAssemblyClose(assemblyHandle);
                this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #10]->>>>>>>>>>>>>");
                this.logger.LogInformation("Successfully Released OpenMod plugin!");
                this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #11]->>>>>>>>>>>>>");
            }
            this.logger.LogCritical("<<<<<<<<<<<<<-[FLAG #12]->>>>>>>>>>>>>");

            assemblyHandles.Clear();

            return UniTask.CompletedTask;
        }


        private async void onLoadAssemblyRequested(IntPtr assemblyHandle, Assembly assembly)
        {
            this.logger.LogCritical("<<<<<<<<<<<<<-[LOADING NEW PLUGIN ASSEMBLY FROM EVENT #1]->>>>>>>>>>>>>");
            Thread.Sleep(1000);
            IOpenModPlugin openModPlugin = await this.pluginActivator.TryActivatePluginAsync(assembly);
            this.logger.LogCritical("<<<<<<<<<<<<<-[LOADING NEW PLUGIN ASSEMBLY FROM EVENT #2]->>>>>>>>>>>>>");
            if (openModPlugin == null)
            {
                this.logger.LogCritical("<<<<<<<<<<<<<-[LOADING NEW PLUGIN ASSEMBLY FROM EVENT #3]->>>>>>>>>>>>>");
                this.logger.LogWarning($"OpenMod Integration didnt work out to load plugin {assembly.GetName().Name}!");
                this.logger.LogCritical("<<<<<<<<<<<<<-[LOADING NEW PLUGIN ASSEMBLY FROM EVENT #4]->>>>>>>>>>>>>");
            }
            else
            {
                this.logger.LogCritical("<<<<<<<<<<<<<-[LOADING NEW PLUGIN ASSEMBLY FROM EVENT #5]->>>>>>>>>>>>>");
                assemblyHandles.Add(assemblyHandle);
                this.logger.LogCritical("<<<<<<<<<<<<<-[LOADING NEW PLUGIN ASSEMBLY FROM EVENT #6]->>>>>>>>>>>>>");
                this.logger.LogInformation($"OpenMod Integration Loaded new plugin: {openModPlugin.DisplayName}!");
                this.logger.LogCritical("<<<<<<<<<<<<<-[LOADING NEW PLUGIN ASSEMBLY FROM EVENT #7]->>>>>>>>>>>>>");
            }
            this.logger.LogCritical("<<<<<<<<<<<<<-[LOADING NEW PLUGIN ASSEMBLY FROM EVENT #8]->>>>>>>>>>>>>");
        }
    }
}
