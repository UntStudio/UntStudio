using System.Net;
using UntStudio.Loader.API;

namespace UntStudio.Loader.Servers
{
    internal class ServerResult
    {
        public RequestResponse Response;

        public Plugin Plugin;

        public HttpStatusCode? HttpStatusCode;



        public ServerResult(RequestResponse response, Plugin plugin)
        {
            Response = response;
            Plugin = plugin;
        }

        public ServerResult(RequestResponse response)
        {
            Response = response;
        }

        public ServerResult(Plugin plugin)
        {
            Plugin = plugin;
        }

        public ServerResult(HttpStatusCode? httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public ServerResult()
        {
        }



        public bool HasResponse => Response != null;

        public bool HasPlugin => Plugin != null;

        public bool HasHttpStatusCode => HttpStatusCode.HasValue;
    }
}
