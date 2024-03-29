﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using UntStudio.External.API;
using UntStudio.Loader.Activators;
using UntStudio.Loader.API;
using UntStudio.Loader.API.Activators;
using UntStudio.Loader.API.Decryptors;
using UntStudio.Loader.API.PortableExecutable;
using UntStudio.Loader.API.Services;
using UntStudio.Loader.Decryptors;
using UntStudio.Loader.Logging;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;
using UntStudio.Loader.Bits;

namespace UntStudio.Loader;

internal static class Loader
{
    internal static void Create(string formattedLicenseKeyPluginsText)
    {
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
        RunAntiDebug();

        ILoaderBuilder builder = new LoaderBuilder();
        builder.Services.AddSingleton<IServer, Server>();
        builder.Services.AddSingleton<ILoaderConfiguration>(new LoaderConfiguration(showPlugins, licenseKey, plugins));
        builder.Services.AddSingleton<IDecryptor, Decryptor>();
        builder.Services.AddSingleton<IPEBit, PEBit>();
        builder.Services.AddSingleton<IMonoActivator, MonoActivator>();
        builder.Services.AddSingleton<IRocketModPluginActivator, RocketModPluginActivator>();
        builder.Services.AddSingleton<IOpenModPluginActivator, OpenModPluginActivator>();
        builder.Services.AddSingleton<IPluginFrameworkActivatorResolver, PluginFrameworkActivatorResolver>();
        builder.AddLogging(new ConsoleLogging());
        IServiceProvider serviceProvider = builder.Build();

        new Startup(serviceProvider);
    }

    internal static void RunAntiDebug()
    {
        Thread thread = new Thread(() =>
        {
            while (true)
            {
                ExternalAntiDebugCalls.HideThreadsInCurrentThread();
                Thread.Sleep(5000);
            }
        });

        thread.Start();
    }
}
