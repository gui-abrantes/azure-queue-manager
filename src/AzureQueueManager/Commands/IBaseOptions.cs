using AzureQueueManager.Services;
using CliFx;
using CliFx.Attributes;

namespace AzureQueueManager.Commands
{
    public interface IBaseOptions : ICommand
    {
        [CommandOption("connectionstring", Description = "Azure Service ConnectionString - NAMESPACE", EnvironmentVariable = "SB_CONNSTR", IsRequired = true)]
        public string ConnectionString
        {
            get => ServiceBusSettings.ConnectionString;
            set => ServiceBusSettings.ConnectionString = value;
        }

        [CommandOption("entity", 'e', Description = "Queue/topic that should be processed", IsRequired = true)]
        public string Entity
        {
            get => ServiceBusSettings.Entity;
            set => ServiceBusSettings.Entity = value;
        }

        [CommandOption("subscription", 's', Description = "Subscription of the topic", IsRequired = false)]
        public string Subscription
        {
            get => ServiceBusSettings.Subscription;
            set => ServiceBusSettings.Subscription = value;
        }

        [CommandOption("prefetchcount", Description = "Local Prefetch Buffer")]
        public int PrefetchCount
        {
            get => ServiceBusSettings.PrefetchCount;
            set => ServiceBusSettings.PrefetchCount = value;
        }

        [CommandOption("messagecount", Description = "Receive Message Count")]
        public int MessageCount
        {
            get => ServiceBusSettings.MessageCount;
            set => ServiceBusSettings.MessageCount = value;
        }

        [CommandOption("timeout", Description = "Receiver timeout in seconds")]
        public int Timeout
        {
            get => ServiceBusSettings.Timeout;
            set => ServiceBusSettings.Timeout = value;
        }
    }
}
