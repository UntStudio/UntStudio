using System;
using System.Reflection;
using UnityEngine;
using UntStudio.Loader.Loaders;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;
using Object = UnityEngine.Object;

namespace UntStudio.Loader
{
    public sealed class Startup
    {
        public Startup(ILoaderConfiguration configuration, IServer server, ILogging logging)
        {
            initializeAsync(configuration, server, logging);
        }



        private async void initializeAsync(ILoaderConfiguration configuration, IServer server, ILogging logging)
        {
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
                                ExternalMonoCalls.MonoAssemblyLoadFrom(imageHandle, string.Empty, out _);
                            }
                        }

                        Type[] types = null;
                        try
                        {
                            Assembly assembly = Assembly.Load(serverResult.Bytes);
                            types = assembly.GetTypes();
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            types = ex.Types;
                        }

                        foreach (Type type in types)
                        {
                            if (type.GetInterface("IRocketPlugin") != null)
                            {
                                GameObject gameObject = new GameObject(Guid.NewGuid().ToString(), type);
                                Object.DontDestroyOnLoad(gameObject);

                                logging.Log($"Successfully loaded plugin: {configuration.Plugins[i]}!");
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
