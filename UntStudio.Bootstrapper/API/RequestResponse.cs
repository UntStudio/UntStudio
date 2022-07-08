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
            None,
            VersionOutdated,
            LicenseKeyValidationFailed,
            NameValidationFailed,
            SubscriptionBannedOrIPNotBindedOrExpiredOrSpecifiedLicenseKeyNotFound,
            SpecifiedLicenseKeyOrIPNotBindedOrNameNotFound,
            SubscriptionBanned,
            SubscriptionExpired,
            SubscriptionBlockedByOwner,
            SubscriptionAlreadyBlocked,
            SubscriptionAlreadyUnblocked,
        }
    }
}
