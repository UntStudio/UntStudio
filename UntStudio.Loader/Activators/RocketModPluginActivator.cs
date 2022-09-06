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
                string.Empty,
            });

/*            Assembly pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.GetName().Name.Equals(name));
            if (pluginAssembly == null)
            {
                logging.LogWarning($"Cannot find plugin {name}.");
            }*/

            Type pluginType = assembly.GetTypes().SingleOrDefault(t => t.GetInterface("IRocketPlugin") != null);
            if (pluginType == null)
            {
                logging.LogWarning($"Given plugin from license server is outdated {assembly.GetName().Name}.");
            }

            addComponentMethodInfo.Invoke(containerGameObject, new object[]
            {
                pluginType,
            });

            Object.DontDestroyOnLoad(containerGameObject);
        }
    }
}
