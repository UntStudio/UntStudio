using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UntStudio.Loader.API;
using UntStudio.Loader.Loaders;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;
using Object = UnityEngine.Object;

namespace UntStudio.Loader
{
    public sealed class Startup
    {
        public Startup(IServiceProvider serviceProvider)
        {
            Loader.Log("Loading Startup NOW!");
            initializeAsync
            (
                (ILoaderConfiguration)serviceProvider.GetService(typeof(ILoaderConfiguration)),
                (IServer)serviceProvider.GetService(typeof(IServer)),
                (ILogging)serviceProvider.GetService(typeof(ILogging))
            );
            Loader.Log("END OF Loading Startup NOW!");
        }


        private async void initializeAsync(ILoaderConfiguration configuration, IServer server, ILogging logging)
        {
            Loader.Log("RIGHT NOW INITIALIZE ASYNC!-_!__!_!_!__!_!_");

            for (int i = 0; i < configuration.Plugins.Length; i++)
            {
                ServerResult serverResult = await server.GetUnloadPluginAsync(configuration.Key, configuration.Plugins[i]);
                if (serverResult.HasBytes)
                {
                    logging.Log($"Loading plugin: {configuration.Plugins[i]}.");
                    try
                    {
                        unsafe
                        {
                            fixed (byte* pointer = serverResult.Bytes)
                            {
                                IntPtr imageHandle = ExternalMonoCalls.MonoImageOpenFromData((IntPtr)pointer, serverResult.Bytes.Length, false, out _);
                                IntPtr assemblyHandle = ExternalMonoCalls.MonoAssemblyLoadFrom(imageHandle, string.Empty, out _);

                                GameObject containerGameObject = new GameObject();
                                MethodInfo createGameObjectMethodInfo = typeof(GameObject).GetMethod("Internal_CreateGameObject",
                                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
                                MethodInfo addComponentMethodInfo = typeof(GameObject).GetMethod("Internal_AddComponentWithType",
                                    BindingFlags.Instance | BindingFlags.NonPublic);

                                createGameObjectMethodInfo.Invoke(null, new object[]
                                {
                                    containerGameObject,
                                    configuration.Plugins[i],
                                });

                                Assembly pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Equals(configuration.Plugins));
                                Type pluginType = pluginAssembly.GetTypes().FirstOrDefault(t => t.GetInterface("IRocketPlugin") != null);

                                addComponentMethodInfo.Invoke(containerGameObject, new object[]
                                {
                                    pluginType,
                                });

                                Object.DontDestroyOnLoad(containerGameObject);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logging.Log($"An error ocurred while loading plugin: {configuration.Plugins[i]}! Error: {ex}");
                        continue;
                    }
                }
            }
        }
    }
}
