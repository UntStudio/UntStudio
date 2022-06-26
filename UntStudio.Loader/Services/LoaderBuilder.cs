using Microsoft.Extensions.DependencyInjection;
using UntStudio.Loader.Logging;

namespace UntStudio.Loader.Services
{
    internal class LoaderBuilder : ILoaderBuilder
    {
        public IServiceCollection Services { get; }



        
        public IServiceCollection AddLogging(ILogging logger)
        {
            Services.AddSingleton(logger);
            return Services;
        }

        public IServiceProvider Build()
        {
            return Services.BuildServiceProvider();
        }
    }
}
