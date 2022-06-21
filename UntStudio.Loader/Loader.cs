using Microsoft.Extensions.DependencyInjection;
using UntStudio.Loader.Servers;
using UntStudio.Loader.Services;

namespace UntStudio.Loader
{
    public sealed class Loader
    {
        public static IServiceProvider Create()
        {
            ILoaderBuilder builder = new LoaderBuilder();

            builder.Services.AddSingleton<IServer, Server>();

            return builder.Build();
        }
    }
}
