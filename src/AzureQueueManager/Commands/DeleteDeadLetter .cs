using AzureQueueManager.Services;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace AzureQueueManager.Commands
{
    [Command("deadletter delete", Description = "Delete dead letter messages")]
    public class DeleteDeadLetter (ServiceBus client) : IBaseOptions
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var cancellationToken = console.RegisterCancellationHandler();

            await DeleteMessages(console, cancellationToken);
        }

        private async Task DeleteMessages(IConsole console, CancellationToken cancellationToken)
        {
            var totalDeleted = 0;

            client.StartClient(true);

            var messageList = await client.ReceiveAsync();

            while (messageList?.Count > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    await client.AbandonMessages(messageList);

                    break;
                }

                var deleteTasks = messageList.Select(async message =>
                {
                    await client.CompleteAsync(message.SystemProperties.LockToken);

                    console.Output.WriteLine($"MessageId: {message.MessageId} deleted");

                    Interlocked.Increment(ref totalDeleted);
                });

                await Task.WhenAll(deleteTasks);

                messageList = await client.ReceiveAsync();
            }

            await client.CloseClient();

            console.Output.WriteLine($"{totalDeleted} messages deleted");
        }
    }
}