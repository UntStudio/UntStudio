using System;
using UntStudio.Server.Models;

namespace UntStudio.Server.Builders
{
    public interface IPluginSubscriptionBuilder
    {
        IPluginSubscriptionBuilder Called(string name);

        IPluginSubscriptionBuilder MakeBanned();

        IPluginSubscriptionBuilder MakeBlockedByOwner();

        IPluginSubscriptionBuilder MakeUnblockedByOwner();

        IPluginSubscriptionBuilder AddKey(string value);

        IPluginSubscriptionBuilder WithAddresses(string values);

        IPluginSubscriptionBuilder WithPurchaseTime(DateTime date);

        IPluginSubscriptionBuilder WithExpireTime(DateTime date);

        IPluginSubscriptionBuilder WillExpireInYears(int value);

        IPluginSubscriptionBuilder WillExpireInDays(int value);

        IPluginSubscriptionBuilder WillExpireIn5Years();

        IPluginSubscriptionBuilder WillExpireIn30Days();

        IPluginSubscriptionBuilder WillExpireIn14Days();

        IPluginSubscriptionBuilder WillExpireIn7Days();

        IPluginSubscriptionBuilder MakeFree();

        PluginSubscription Build();
    }
}
