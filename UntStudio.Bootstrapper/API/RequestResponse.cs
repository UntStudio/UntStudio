namespace UntStudio.Bootstrapper.API
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
            None = 0,
            VersionOutdated = 1,
            KeyValidationFailed = 2,
            NameValidationFailed = 3,
            SubscriptionBannedOrExpiredOrSpecifiedKeyNotFound = 4,
            IPNotBindedOrSpecifiedKeyOrNameNotFound = 5,
            SubscriptionBanned = 6,
            SubscriptionExpired = 7,
        }
    }
}
