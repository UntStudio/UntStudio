using Microsoft.Extensions.DependencyInjection;

namespace UntStudio.Loader.Services
{
    internal class LoaderBuilder : ILoaderBuilder
    {
        public IServiceCollection Services { get; }



        public IServiceProvider Build()
        {
            return Services.BuildServiceProvider();
        }
    }
}
