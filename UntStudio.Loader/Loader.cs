using Microsoft.Extensions.DependencyInjection;
using System;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;

namespace UntStudio.Loader
{
    public class Loader
    {
        public static void Log(string message)
        {
            message = $"{DateTime.Now}:{message}";
            Console.WriteLine("Log: " + message);
        }

        public static void /*IServiceProvider*/ Create(/*string formattedKeyPluginsText*/)
        {
            Run();
        }

        public static void Run()
        {
            try
            {
                Log("FLAG #1");
                ILoaderBuilder builder = new LoaderBuilder();
                Log("FLAG #2");

                /*string splittedParsedKey = formattedKeyPluginsText.Split(';')[0];
                Log("FLAG #3");
                string[] splittedParsedPlugins = formattedKeyPluginsText
                    .Replace(splittedParsedKey, string.Empty)
                    .Replace(";", string.Empty)
                    .Split(',');*/
                Log("FLAG #4");

                builder.Services.AddSingleton<IServer, Server>();
                Log("FLAG #5");

                //builder.Services.AddSingleton<ILoaderConfiguration>(new LoaderConfiguration(splittedParsedKey, splittedParsedPlugins));
                builder.Services.AddSingleton<ILoaderConfiguration>(new LoaderConfiguration("1234-1234-1234-1234", new string[]
                {
                    "PluginTestUnt",
                }));
                Log("FLAG #6");

                builder.AddLogging(new ConsoleLogging());
                Log("FLAG #7");

                IServiceProvider serviceProvider = builder.Build();
                Startup startup = new Startup(serviceProvider);
                Log("FLAG #8");
                //return builder.Build();
            }
            catch (Exception ex)
            {
                Log("FLAG #9, ERRORE: " + ex);
            }

            //return null;
        }
    }
}
