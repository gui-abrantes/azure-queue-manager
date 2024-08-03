using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus;

namespace AzureQueueManager.Services
{
    public class ServiceBus
    {
        private bool _isTopic => !string.IsNullOrWhiteSpace(ServiceBusSettings.Subscription);

        private QueueClient? _queueClient;

        private TopicClient? _topicClient;

        private MessageReceiver? _receiver;


        public void StartClient(bool forDeadLetter = false)
        {
            if (_isTopic)
            {
                _topicClient = new TopicClient(ServiceBusSettings.ConnectionString, ServiceBusSettings.Entity);
            }
            else
            {
                _queueClient = new QueueClient(ServiceBusSettings.ConnectionString, ServiceBusSettings.Entity);
            }

            BuildMessageReceiver(forDeadLetter);
        }

        public Task CloseClient()
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

        public Task<IList<Message>> PeekAsync()
        {
            return _receiver!.PeekAsync(ServiceBusSettings.MessageCount);
        }

        public Task<IList<Message>> ReceiveAsync()
        {
            return _receiver!.ReceiveAsync(ServiceBusSettings.MessageCount, TimeSpan.FromSeconds(ServiceBusSettings.Timeout));
        }

        public Task CompleteAsync(string lockToken)
        {
            return _receiver!.CompleteAsync(lockToken);
        }

        public Task SendMessage(Message message)
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

        public Task CancelScheduledMessage(Message message)
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

        public async Task AbandonMessages(IList<Message> messages)
        {
            var abandonMessages = messages.Select(async message =>
            {
                await _receiver.AbandonAsync(message.SystemProperties.LockToken);
            });

            await Task.WhenAll(abandonMessages);
        }

        private void BuildMessageReceiver(bool forDeadLetter)
        {
            var entityPath = ServiceBusSettings.Entity;

            if (_isTopic)
            {
                entityPath = EntityNameHelper.FormatSubscriptionPath(ServiceBusSettings.Entity, ServiceBusSettings.Subscription);
            }

            if (forDeadLetter)
            {
                entityPath = EntityNameHelper.FormatDeadLetterPath(entityPath);
            }

            _receiver = new MessageReceiver(ServiceBusSettings.ConnectionString, entityPath, ReceiveMode.PeekLock, RetryPolicy.Default, ServiceBusSettings.PrefetchCount);
        }
    }
}