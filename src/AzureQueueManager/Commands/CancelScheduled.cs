using AzureQueueManager.Services;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace AzureQueueManager.Commands
{
    [Command("scheduled cancel", Description = "Cancel scheduled messages")]
    public class CancelScheduled (ServiceBus client) : IBaseOptions
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var totalCanceled = 0;

            client.StartClient();

            var messageList = await client.PeekAsync();

            while (messageList?.Count > 0)
            {
                var cancelTasks = messageList.Select(async message =>
                {
                    await client.CancelScheduledMessage(message);

                    console.Output.WriteLine($"MessageId: {message.MessageId} canceled");

                    Interlocked.Increment(ref totalCanceled);
                });

                await Task.WhenAll(cancelTasks);

                messageList = await client.PeekAsync();
            }

            await client.CloseClient();

            console.Output.WriteLine($"{totalCanceled} scheduled messages canceled");
        }
    }
}