using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UntStudio.Loader.API;
using UntStudio.Loader.Loaders;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;
using static UntStudio.Loader.API.RequestResponse;
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
                if (serverResult.HasResponse)
                {
                    translateServerResponse(serverResult.Response.Code);
                }
                if (serverResult.HasBytes)
                {
                    logging.Log($"Loading plugin: {configuration.Plugins[i]}.");
                    try
                    {
                        logging.Log("Loading plugin, flag#1");
                        unsafe
                        {
                            logging.Log("Loading plugin, flag#2");

                            fixed (byte* pointer = serverResult.Bytes)
                            {
                                logging.Log("Loading plugin, flag#3");

                                IntPtr imageHandle = ExternalMonoCalls.MonoImageOpenFromData((IntPtr)pointer, serverResult.Bytes.Length, false, out _);
                                logging.Log("Loading plugin, flag#4");

                                IntPtr assemblyHandle = ExternalMonoCalls.MonoAssemblyLoadFrom(imageHandle, string.Empty, out _);
                                logging.Log("Loading plugin, flag#5");


                                GameObject containerGameObject = new GameObject();
                                logging.Log("Loading plugin, flag#6");

                                MethodInfo createGameObjectMethodInfo = typeof(GameObject).GetMethod("Internal_CreateGameObject",
                                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
                                logging.Log("Loading plugin, flag#7");

                                MethodInfo addComponentMethodInfo = typeof(GameObject).GetMethod("Internal_AddComponentWithType",
                                    BindingFlags.Instance | BindingFlags.NonPublic);

                                logging.Log("Loading plugin, flag8");

                                createGameObjectMethodInfo.Invoke(null, new object[]
                                {
                                    containerGameObject,
                                    configuration.Plugins[i],
                                });

                                logging.Log("Loading plugin, flag#9");
                                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                                /*for (int j = 0; j < assemblies.Length; j++)
                                {
                                    logging.Log("------------------------");
                                    logging.Log("assmelby1.FullName: " + assemblies[j].FullName);
                                    logging.Log("assmelby1.GetName().FullName: " + assemblies[j].GetName().FullName);
                                    logging.Log("assmelby1.GetName().Name: " + assemblies[j].GetName().Name);
                                    // ERROR WHEN SHOWING any info about assembly which loaded in mono
                                    //logging.Log("assmelby1.GetName().CultureName: " + assemblies[j].GetName().CultureName);
                                    logging.Log("------------------------");
                                }*/
                                Assembly pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Equals(configuration.Plugins[i]));
                                logging.Log("Loading plugin, flag#10");

                                logging.Log(pluginAssembly == null ? "PLUGIN ASSEMBLY IS NULL, NOT FOUND!" : "not null, good!");

                                Type pluginType = pluginAssembly.GetTypes().FirstOrDefault(t => t.GetInterface("IRocketPlugin") != null);
                                logging.Log("Types of found assembly: ");
                                /*foreach (var item in pluginAssembly.GetTypes())
                                {
                                    logging.Log("------------------------");
                                    logging.Log("Type: " + item.Name);
                                    logging.Log("IsRocketPLugin: " + item.GetInterface("IRocketPlugin") != null ? "Found" : "NOT FOUND");
                                    logging.Log("------------------------");
                                }*/
                                logging.Log(pluginType == null ? "PLUGIN type IS NULL, NOT FOUND!" : "not null, good!");
                                logging.Log("Loading plugin, flag#11");

                                addComponentMethodInfo.Invoke(containerGameObject, new object[]
                                {
                                    pluginType,
                                });
                                logging.Log("Loading plugin, flag#12");

                                Object.DontDestroyOnLoad(containerGameObject);
                                logging.Log("Loading plugin, flag#13");

                            }
                            logging.Log("Loading plugin, flag#14");

                        }
                        logging.Log("Loading plugin, flag#15");

                    }
                    catch (Exception ex)
                    {
                        logging.Log($"An error ocurred while loading plugin: {configuration.Plugins[i]}! Error: {ex}");
                        continue;
                    }
                }
            }
        }

        private string translateServerResponse(CodeResponse code)
        {
            return code switch
            {
                CodeResponse.None                                                           => "Nothing.",
                CodeResponse.VersionOutdated                                                => "Loader version outdated, please download latest!",
                CodeResponse.KeyValidationFailed                                            => "Please, check your key, and write it properly!",
                CodeResponse.NameValidationFailed                                           => "Plugin name validation failed, please verify your plugin configuration.",
                CodeResponse.SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedKeyNotFound => "Your subscription banned or IP not binded or expired or specified key not found.",
                CodeResponse.SpecifiedKeyOrIPNotBindedOrNameNotFound                        => "Your key is not binded or key does not exist or plugin name not found.",
                CodeResponse.SubscriptionBanned                                             => "Your subscription was banned.",
                CodeResponse.SubscriptionExpired                                            => "Your subscription was expired.",
                CodeResponse.SubscriptionBlockedByOwner                                     => "Your subscription was blocked by yourself, and cannot be used.",
                CodeResponse.SubscriptionAlreadyBlocked                                     => "Your subscription was already blocked by yourself.",
                CodeResponse.SubscriptionAlreadyUnblocked                                   => "Your subscription was already unblocked by yourself.",
                _ => "Unknown server response, please contact with Administrator, may version is outdated.",
            };
        }
    }
}
