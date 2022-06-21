using Microsoft.Extensions.DependencyInjection;

namespace UntStudio.Loader.Services
{
    internal interface ILoaderBuilder
    {
        IServiceCollection Services { get; }

        IServiceProvider Build();
    }
}