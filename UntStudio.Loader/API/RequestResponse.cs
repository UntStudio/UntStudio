namespace UntStudio.Loader.API
{
    public class RequestResponse
    {
        public readonly CodeResponse Code;



        public RequestResponse(CodeResponse code)
        {
            Code = code;
        }

        public RequestResponse()
        {
        }



        public enum CodeResponse
        {
            VersionOutdated,
            KeyValidationFailed,
            NotFound,
            SubscriptionExpired,
            NotFoundOrSubscriptionExpired,
        }
    }
}
