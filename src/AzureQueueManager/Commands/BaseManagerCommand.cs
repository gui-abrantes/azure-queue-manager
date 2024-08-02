using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace AzureQueueManager.Commands
{
    public abstract class BaseManagerCommand : ICommand
    {
        [CommandOption("entity", 'e', Description = "Queue/topic that should be processed", IsRequired = true)]
        public required string Entity { get; init; }

        [CommandOption("subscription", 's', Description = "Subscription of the topic", IsRequired = false)]
        public string Subscription { get; init; } = "";

        [CommandOption("prefetchcount", Description = "Local Prefetch Buffer")]
        public int PrefetchCount { get; init; } = 1000;

        [CommandOption("messagecount", Description = "Receive Message Count")]
        public int MessageCount { get; init; } = 100;

        [CommandOption("timeout", Description = "Receiver timeout in seconds")]
        public int Timeout { get; init; } = 10;

        [CommandOption("connectionstring", Description = "Azure Service ConnectionString - NAMESPACE", EnvironmentVariable = "SB_CONNSTR")]
        public required string ConnectionString { get; init; }

        public abstract ValueTask ExecuteAsync(IConsole console);



        private bool _isTopic => !string.IsNullOrWhiteSpace(Subscription);

        private IQueueClient? _queueClient;

        private ITopicClient? _topicClient;

        protected IMessageReceiver BuildMessageReceiver(bool fromDeadLetter = false)
        {
            var entityPath = Entity;

            if (_isTopic)
            {
                entityPath = EntityNameHelper.FormatSubscriptionPath(Entity, Subscription);
            }

            if (fromDeadLetter)
            {
                entityPath = EntityNameHelper.FormatDeadLetterPath(entityPath);
            }

            return new MessageReceiver(ConnectionString, entityPath, ReceiveMode.PeekLock, RetryPolicy.Default, PrefetchCount);
        }

        protected void StartClient()
        {
            if (_isTopic)
            {
                _topicClient = new TopicClient(ConnectionString, Entity);
            }
            else
            {
                _queueClient = new QueueClient(ConnectionString, Entity);
            }
        }

        protected Task CloseClient()
        {
            if (_isTopic)
            {
                return _topicClient!.CloseAsync();
            }
            else
            {
                return _queueClient!.CloseAsync();
            }
        }

        protected Task SendMessage(Message message)
        {
            if (_isTopic)
            {
                return _topicClient!.SendAsync(message);
            }
            else
            {
                return _queueClient!.SendAsync(message);
            }
        }

        protected Task CancelScheduledMessage(Message message)
        {
            if (message.ScheduledEnqueueTimeUtc <= DateTime.UtcNow)
            {
                return Task.CompletedTask;
            }

            try
            {
                if (_isTopic)
                {
                    return _topicClient!.CancelScheduledMessageAsync(message.SystemProperties.SequenceNumber);
                }
                else
                {
                    return _queueClient!.CancelScheduledMessageAsync(message.SystemProperties.SequenceNumber);
                }
            }
            catch (Exception) 
            {
                return Task.CompletedTask;
            }
        }

        protected async Task AbandonMessages(IList<Message> messages, IMessageReceiver receiver)
        {
            var abandonMessages = messages.Select(async message =>
            {
                await receiver.AbandonAsync(message.SystemProperties.LockToken);
            });

            await Task.WhenAll(abandonMessages);
        }
    }
}
