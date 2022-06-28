namespace UntStudio.Loader.API
{
    public class RequestResponse
    {
        public CodeResponse Code;



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
            NameValidationFailed,
            SubscriptionBannedOrExpiredOrSpecifiedKeyNotFound,
            IPNotBindedOrSpecifiedKeyOrNameNotFound,
            SubscriptionBanned,
            SubscriptionExpired,
        }
    }
}
