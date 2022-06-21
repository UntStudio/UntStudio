namespace UntStudio.Loader.API
{
    internal class PluginRequestResult
    {
        public PluginRequestResult(CodeResponse response)
        {
            Response = response;
        }



        public CodeResponse Response { get; set; }

        public enum CodeResponse
        {
            NotFound,
            SubscriptionExpired,
        }
    }
}
