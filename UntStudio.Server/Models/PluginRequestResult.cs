namespace UntStudio.Server.Models
{
    public class PluginRequestResult
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
