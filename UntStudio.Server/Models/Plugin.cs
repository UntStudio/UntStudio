using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace UntStudio.Server.Models
{
    public class Plugin
    {
        public Plugin(string name, string key, string allowedAddresses, DateTime purchaseTime, DateTime expirationTime)
        {
            Name = name;
            Key = key;
            PurchaseTime = DateTime.ParseExact(purchaseTime.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
            ExpirationTime = DateTime.ParseExact(expirationTime.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
            AllowedAddresses = allowedAddresses;
        }

        public Plugin(string name, string key, string allowedAddresses, DateTime expirationTime) 
            : this(name, key, allowedAddresses, DateTime.Now, expirationTime)
        {
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

        public string AllowedAddresses { get; set; }

        public string[] AllowedAddressesParsed => AllowedAddresses.Split(", ");

        [Required]
        public DateTime PurchaseTime { get; set; }

        [Required]
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
