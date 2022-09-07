using SDG.Unturned;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UntStudio.Loader.Logging;
using Object = UnityEngine.Object;

namespace UntStudio.Loader.Activators
{
    public sealed class RocketModPluginActivator : IRocketModPluginActivator
    {
        private readonly ILogging logging;

        public RocketModPluginActivator(ILogging logging)
        {
            this.logging = logging;
        }


        public void Activate(IntPtr assemblyHandle, Assembly assembly)
        {
            GameObject containerGameObject = new GameObject();
            MethodInfo createGameObjectMethodInfo = typeof(GameObject).GetMethod("Internal_CreateGameObject",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo addComponentMethodInfo = typeof(GameObject).GetMethod("Internal_AddComponentWithType",
                BindingFlags.Instance | BindingFlags.NonPublic);

            createGameObjectMethodInfo.Invoke(null, new object[]
            {
                containerGameObject,
                //string.Empty,
                assembly.GetName().Name
            });

            Type[] types = null;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            Type pluginType = types.FirstOrDefault(t => t.GetInterface("IRocketPlugin") != null);
            if (pluginType == null)
            {
                logging.LogWarning($"Plugin from license server is outdated {assembly.GetName().Name}.");
                return;
            }

            addComponentMethodInfo.Invoke(containerGameObject, new object[]
            {
                pluginType,
            });

            Object.DontDestroyOnLoad(containerGameObject);
            PluginAdvertising.Get().AddPlugin(assembly.GetName().Name);
        }
    }
}
