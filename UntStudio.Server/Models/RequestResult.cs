namespace UntStudio.Server.Models;

public class RequestResult
{
    public RequestResult(CodeResponse response)
    {
        Response = response;
    }



    public CodeResponse Response { get; set; }

    public enum CodeResponse
    {
        VersionOutdated,
        NotFound,
        SubscriptionExpired,
        NotFoundOrSubscriptionExpired,
    }
}
