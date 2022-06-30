using System;
using UntStudio.Server.Models;

namespace UntStudio.Server.Builders
{
    public sealed class PluginSubscriptionBuilder : IPluginSubscriptionBuilder
    {
        private readonly PluginSubscription plugin;



        public PluginSubscriptionBuilder(PluginSubscription plugin)
        {
            this.plugin = plugin;
        }



        public IPluginSubscriptionBuilder AddKey(string value)
        {
            throw new NotImplementedException();
        }

        

        public IPluginSubscriptionBuilder Called(string name)
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder MakeBanned()
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder MakeBlockedByOwner()
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder MakeFree()
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder MakeUnblockedByOwner()
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder WillExpireIn14Days()
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder WillExpireIn30Days()
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder WillExpireIn5Years()
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder WillExpireIn7Days()
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder WillExpireInDays(int value)
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder WillExpireInYears(int value)
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder WithAddresses(string values)
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder WithExpireTime(DateTime date)
        {
            throw new NotImplementedException();
        }

        public IPluginSubscriptionBuilder WithPurchaseTime(DateTime date)
        {
            throw new NotImplementedException();
        }

        public PluginSubscription Build()
        {
            throw new NotImplementedException();
        }
    }
}
