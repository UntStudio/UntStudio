using System;
using System.ComponentModel.DataAnnotations;

namespace UntStudio.Server.Models
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

        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(19, MinimumLength = 19)]
        public string Key { get; set; }

        public DateTime PurchaseTime { get; set; }

        public DateTime ExpirationTime { get; set; }

        public bool Expired => (ExpirationTime - DateTime.Now).TotalMilliseconds <= 0;

        public bool NotExpired => Expired == false;



        public override string ToString()
        {
            TimeSpan timeSpan = ExpirationTime - DateTime.Now; 
            return $"[Plugin: {Name}, Key: {Key}], Purchased: {PurchaseTime}, Is Expired: {Expired}, Expires in: {timeSpan.Days}d. {timeSpan.Minutes}m. {timeSpan.Seconds}s.";
        }
    }
}
