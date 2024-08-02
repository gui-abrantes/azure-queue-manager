using CliFx.Attributes;
using CliFx.Infrastructure;

namespace AzureQueueManager.Commands
{
    [Command("deadletter resend", Description = "Resend dead letter messages")]
    public class ResendDeadLetter : BaseManagerCommand
    {
        public override async ValueTask ExecuteAsync(IConsole console)
        {
            var cancellationToken = console.RegisterCancellationHandler();

            await DeleteMessages(console, cancellationToken);
        }

        private async Task DeleteMessages(IConsole console, CancellationToken cancellationToken)
        {
            var totalProcessed = 0;

            StartClient();

            var receiver = BuildMessageReceiver(true);

            var messageList = await receiver.ReceiveAsync(MessageCount, TimeSpan.FromSeconds(Timeout));

            while (messageList?.Count > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    await AbandonMessages(messageList, receiver);

                    break;
                }

                var processTasks = messageList.Select(async message =>
                {
                    var cloneMessage = message.Clone();

                    await SendMessage(cloneMessage);

                    await receiver.CompleteAsync(message.SystemProperties.LockToken);

                    console.Output.WriteLine($"MessageId: {message.MessageId} resended");

                    Interlocked.Increment(ref totalProcessed);
                });

                await Task.WhenAll(processTasks);

                messageList = await receiver.ReceiveAsync(MessageCount, TimeSpan.FromSeconds(Timeout));
            }

            await CloseClient();

            console.Output.WriteLine($"{totalProcessed} messages resended");
        }
    }
}