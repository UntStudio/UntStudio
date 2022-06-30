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
            None,
            VersionOutdated,
            KeyValidationFailed,
            NameValidationFailed,
            SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedKeyNotFound,
            SpecifiedKeyOrIPNotBindedOrNameNotFound,
            SubscriptionBanned,
            SubscriptionExpired,
            SubscriptionBlockedByOwner,
            SubscriptionAlreadyBlocked,
            SubscriptionAlreadyUnblocked,
        }
    }
}
