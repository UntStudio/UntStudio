using Microsoft.Extensions.DependencyInjection;
using System;

namespace UntStudio.Loader.API.Services;

public interface ILoaderBuilder
{
    IServiceCollection Services { get; }

    IServiceCollection AddLogging(ILogging logger);

    IServiceProvider Build();
}