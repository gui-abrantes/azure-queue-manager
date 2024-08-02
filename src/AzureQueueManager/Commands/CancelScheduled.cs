using CliFx.Attributes;
using CliFx.Infrastructure;

namespace AzureQueueManager.Commands
{
    [Command("scheduled cancel", Description = "Cancel scheduled messages")]
    public class CancelScheduled : BaseManagerCommand
    {
        public override async ValueTask ExecuteAsync(IConsole console)
        {
            var totalCanceled = 0;

            StartClient();

            var receiver = BuildMessageReceiver();

            var messageList = await receiver.PeekAsync(MessageCount);

            while (messageList?.Count > 0)
            {
                var cancelTasks = messageList.Select(async message =>
                {
                    await CancelScheduledMessage(message);

                    console.Output.WriteLine($"MessageId: {message.MessageId} canceled");

                    Interlocked.Increment(ref totalCanceled);
                });

                await Task.WhenAll(cancelTasks);

                messageList = await receiver.PeekAsync(MessageCount);
            }

            await CloseClient();

            console.Output.WriteLine($"{totalCanceled} scheduled messages canceled");
        }
    }
}