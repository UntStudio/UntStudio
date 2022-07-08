using Microsoft.Extensions.DependencyInjection;
using System;
using UntStudio.Loader.External;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;

namespace UntStudio.Loader;

internal static class Loader
{
    internal static void Create(string formattedLicenseKeyPluginsText)
    {
        ExternalAntiDebugCalls.HideThreadsInCurrentThread();

        string[] parsedShowPluginsAndKey = formattedLicenseKeyPluginsText.Split(';');
        bool showPlugins = bool.Parse(parsedShowPluginsAndKey[0]);
        string licenseKeyParsed = parsedShowPluginsAndKey[1];
        string[] pluginsParsed = formattedLicenseKeyPluginsText
            .Replace(licenseKeyParsed, string.Empty)
            .Replace(parsedShowPluginsAndKey[0], string.Empty)
            .Replace(";", string.Empty)
            .Split(',');

        Run(showPlugins, licenseKeyParsed, pluginsParsed);
    }

    internal static void Run(bool showPlugins, string licenseKey, string[] plugins)
    {
        ILoaderBuilder builder = new LoaderBuilder();
        builder.Services.AddSingleton<IServer, Server>();
        builder.Services.AddSingleton<ILoaderConfiguration>(new LoaderConfiguration(showPlugins, licenseKey, plugins));
        builder.AddLogging(new ConsoleLogging());
        IServiceProvider serviceProvider = builder.Build();

        new Startup(serviceProvider);
    }
}
