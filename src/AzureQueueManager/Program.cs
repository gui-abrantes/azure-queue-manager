using AzureQueueManager.Commands;
using CliFx;

await new CliApplicationBuilder()
            .AddCommand<ResendDeadLetter>()
            .AddCommand<DeleteDeadLetter>()
            .AddCommand<CancelScheduled>()
            .SetExecutableName("azmsgctl")
            .Build()
            .RunAsync();