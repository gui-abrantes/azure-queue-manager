using CliFx.Attributes;
using CliFx.Infrastructure;

namespace AzureQueueManager.Commands
{
    [Command("deadletter delete", Description = "Delete dead letter messages")]
    public class DeleteDeadLetter : BaseManagerCommand
    {
        public override async ValueTask ExecuteAsync(IConsole console)
        {
            var cancellationToken = console.RegisterCancellationHandler();

            await DeleteMessages(console, cancellationToken);
        }

        private async Task DeleteMessages(IConsole console, CancellationToken cancellationToken)
        {
            var totalDeleted = 0;

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

                var deleteTasks = messageList.Select(async message =>
                {
                    await receiver.CompleteAsync(message.SystemProperties.LockToken);

                    console.Output.WriteLine($"MessageId: {message.MessageId} deleted");

                    Interlocked.Increment(ref totalDeleted);
                });

                await Task.WhenAll(deleteTasks);

                messageList = await receiver.ReceiveAsync(MessageCount, TimeSpan.FromSeconds(Timeout));
            }

            await CloseClient();

            console.Output.WriteLine($"{totalDeleted} messages deleted");
        }
    }
}