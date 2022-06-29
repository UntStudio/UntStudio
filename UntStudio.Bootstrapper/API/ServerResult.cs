using System.Net;
using UntStudio.Bootstrapper.Models;
using static UntStudio.Bootstrapper.API.RequestResponse;

namespace UntStudio.Bootstrapper.API
{
    internal sealed class ServerResult
    {
        public RequestResponse Response;

        public LoaderEntryPoint LoaderEntryPoint;

        public HttpStatusCode? HttpStatusCode;

        public byte[] Bytes;



        public ServerResult(RequestResponse response)
        {
            Response = response;
        }

        public ServerResult(LoaderEntryPoint loaderEntryPoint)
        {
            LoaderEntryPoint = loaderEntryPoint;
        }

        public ServerResult(HttpStatusCode? httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public ServerResult(byte[] bytes)
        {
            Bytes = bytes;
        }

        public ServerResult()
        {
        }



        public bool HasResponse => Response != null && Response.Code != CodeResponse.None;

        public bool HasHttpStatusCode => HttpStatusCode.HasValue;

        public bool HasBytes => Bytes != null && Bytes.Length > 0;

        public bool HasLoaderEntryPoint => LoaderEntryPoint != null;
    }
}
