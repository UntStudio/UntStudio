﻿using System.Net;
using static UntStudio.Loader.API.Models.RequestResponse;

namespace UntStudio.Loader.API.Models;

public class ServerResult
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



    public bool HasResponse => Response != null && Response.Code != CodeResponse.None;

    public bool HasHttpStatusCode => HttpStatusCode.HasValue;

    public bool HasBytes => Bytes != null && Bytes.Length > 0;
}
