using Microsoft.Extensions.DependencyInjection;
using UntStudio.Loader.Logging;

namespace UntStudio.Loader.Services
{
    internal interface ILoaderBuilder
    {
        IServiceCollection Services { get; }

        IServiceCollection AddLogging(ILogging logger);

        IServiceProvider Build();
    }
}