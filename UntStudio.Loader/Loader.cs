using Microsoft.Extensions.DependencyInjection;
using System;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;

namespace UntStudio.Loader
{
    public class Loader
    {
        public static void Create(string formattedKeyPluginsText)
        {
            string[] parsedShowPluginsAndKey = formattedKeyPluginsText.Split(';');
            bool showPlugins = bool.Parse(parsedShowPluginsAndKey[0]);
            string keyParsed = parsedShowPluginsAndKey[1];
            string[] pluginsParsed = formattedKeyPluginsText
                .Replace(keyParsed, string.Empty)
                .Replace(parsedShowPluginsAndKey[0], string.Empty)
                .Replace(";", string.Empty)
                .Split(',');

            Run(showPlugins, keyParsed, pluginsParsed);
        }

        public static void Run(bool showPlugins, string key, string[] plugins)
        {
            ILoaderBuilder builder = new LoaderBuilder();
            builder.Services.AddSingleton<IServer, Server>();
            builder.Services.AddSingleton<ILoaderConfiguration>(new LoaderConfiguration(showPlugins, key, plugins));
            builder.AddLogging(new ConsoleLogging());
            IServiceProvider serviceProvider = builder.Build();

            new Startup(serviceProvider);
        }
    }
}
