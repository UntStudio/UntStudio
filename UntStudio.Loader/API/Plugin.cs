namespace UntStudio.Loader.API
{
    public class Plugin
    {
        public Plugin(string name, string key, DateTime purchaseTime, DateTime expirationTime)
        {
            Name = name;
            Key = key;
            PurchaseTime = purchaseTime;
            ExpirationTime = expirationTime;
        }

        public Plugin()
        {
        }



        public int Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public DateTime PurchaseTime { get; set; }

        public DateTime ExpirationTime { get; set; }

        public bool Expired => (ExpirationTime - DateTime.Now).TotalMilliseconds <= 0;



        public override string ToString()
        {
            TimeSpan timeSpan = ExpirationTime - DateTime.Now;
            return $"[Plugin: {Name}, Key: {Key}], Purchased: {PurchaseTime}, Is Expired: {Expired}, Expires in: {timeSpan.Days}d. {timeSpan.Minutes}m. {timeSpan.Seconds}s.";
        }
    }
}
