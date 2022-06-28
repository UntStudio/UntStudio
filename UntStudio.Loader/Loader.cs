using Microsoft.Extensions.DependencyInjection;
using System;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;

namespace UntStudio.Loader
{
    public sealed class Loader
    {
        public IServiceProvider Create(string formattedKeyPluginsText)
        {
            ILoaderBuilder builder = new LoaderBuilder();

            string splittedParsedKey = formattedKeyPluginsText.Split(';')[0];
            string[] splittedParsedPlugins = formattedKeyPluginsText
                .Replace(splittedParsedKey, string.Empty)
                .Replace(";", string.Empty)
                .Split(',');

            builder.Services.AddSingleton<IServer, Server>();
            builder.Services.AddSingleton<ILoaderConfiguration>(new LoaderConfiguration(splittedParsedKey, splittedParsedPlugins));
            builder.AddLogging(new ConsoleLogging());
            builder.Services.AddSingleton(typeof(Startup));

            return builder.Build();
        }
    }
}
