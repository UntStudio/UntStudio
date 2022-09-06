using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using UntStudio.Server.Knowns;

namespace UntStudio.Server.Models;

public class PluginSubscription
{
    public PluginSubscription(string name, string key, string allowedAddresses, DateTime purchaseTime, DateTime expirationTime)
    {
        Name = name;
        LicenseKey = key;
        PurchaseTime = DateTime.ParseExact(purchaseTime.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
        ExpirationTime = DateTime.ParseExact(expirationTime.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
        AllowedAddresses = allowedAddresses;
    }

    public PluginSubscription(string name, string key, string allowedAddresses, DateTime expirationTime) 
        : this(name, key, allowedAddresses, DateTime.Now, expirationTime)
    {
    }

    public PluginSubscription(string name, string key, string allowedAddresses) : this(name, key, allowedAddresses, DateTime.Now.AddYears(5))
    {
    }

    public PluginSubscription()
    {
    }



    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [StringLength(KnownPluginKeyLenghts.Lenght, MinimumLength = KnownPluginKeyLenghts.Lenght)]
    public string LicenseKey { get; set; }

    [Required]
    public string AllowedAddresses { get; set; }

    public string[] AllowedAddressesParsed => AllowedAddresses.Split(',');

    [Required]
    public DateTime PurchaseTime { get; set; }

    [Required]
    public DateTime ExpirationTime { get; set; }

    public bool Free { get; set; }

    public bool Banned { get; set; }

    public bool BlockedByOwner { get; set; }

    public bool UnblockedByOwner => BlockedByOwner == false;

    public bool NotBanned => Banned == false;

    public bool Expired => (ExpirationTime - DateTime.Now).TotalMilliseconds <= 0;

    public bool NotExpired => Expired == false;

    public bool NotBannedOrExpired => NotBanned && NotExpired;



    public override string ToString()
    {
        TimeSpan leftTimeForExpire = ExpirationTime - DateTime.Now;
        return new StringBuilder()
            .Append($"[{Name}>{LicenseKey}]")
            .Append($"\nId: {Id}")
            .Append($"\nPurchased: {PurchaseTime}")
            .Append($"\nExpired: {(Expired ? "Yes" : "No")}")
            .Append($"\nAllowed Addresses: {AllowedAddresses}")
            .Append($"\nExpires ({ExpirationTime}) in: {leftTimeForExpire.Days}d.")
            .Append($"\nBlocked: {(Banned ? "Yes" : "No")}")
            .Append($"\nFree: {(Free ? "Yes" : "No")}")
            .ToString();
    }

    public bool SetBan()
    {
        return Banned = true;
    }

    public bool SetUnban()
    {
        return Banned = false;
    }

    public bool SetFree()
    {
        return Free = true;
    }

    public bool SetBlockedByOwner()
    {
        return BlockedByOwner = true;
    }

    public bool SetUnblockedByOwner()
    {
        return BlockedByOwner = false;
    }
}
