using System.Net;

namespace UntStudio.Bootstrapper.API
{
    internal class ServerResult
    {
        public RequestResponse Response;

        public HttpStatusCode? HttpStatusCode;

        public byte[] Bytes;



        public ServerResult(RequestResponse response)
        {
            Response = response;
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



        public bool HasResponse => Response != null;

        public bool HasHttpStatusCode => HttpStatusCode.HasValue;

        public bool HasBytes => Bytes != null;
    }
}
