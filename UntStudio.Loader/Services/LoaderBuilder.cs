using Microsoft.Extensions.DependencyInjection;
using UntStudio.Loader.Logging;

namespace UntStudio.Loader.Services
{
    internal sealed class LoaderBuilder : ILoaderBuilder
    {
        public IServiceCollection Services { get; }



        public IServiceCollection AddLogging(ILogging logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Services.AddSingleton(logger);
            return Services;
        }

        public IServiceProvider Build()
        {
            return Services.BuildServiceProvider();
        }
    }
}
