using AzureQueueManager.Services;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace AzureQueueManager.Commands
{
    [Command("deadletter resend", Description = "Resend dead letter messages")]
    public class ResendDeadLetter (ServiceBus client) : IBaseOptions
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var cancellationToken = console.RegisterCancellationHandler();

            await DeleteMessages(console, cancellationToken);
        }

        private async Task DeleteMessages(IConsole console, CancellationToken cancellationToken)
        {
            var totalProcessed = 0;

            client.StartClient(true);

            var messageList = await client.ReceiveAsync();

            while (messageList?.Count > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    await client.AbandonMessages(messageList);

                    break;
                }

                var processTasks = messageList.Select(async message =>
                {
                    var cloneMessage = message.Clone();

                    await client.SendMessage(cloneMessage);

                    await client.CompleteAsync(message.SystemProperties.LockToken);

                    console.Output.WriteLine($"MessageId: {message.MessageId} resended");

                    Interlocked.Increment(ref totalProcessed);
                });

                await Task.WhenAll(processTasks);

                messageList = await client.ReceiveAsync();
            }

            await client.CloseClient();

            console.Output.WriteLine($"{totalProcessed} messages resended");
        }
    }
}