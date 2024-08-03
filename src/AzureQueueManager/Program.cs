using AzureQueueManager.Commands;
using AzureQueueManager.Services;
using CliFx;
using Microsoft.Extensions.DependencyInjection;

await new CliApplicationBuilder()
            .AddCommand<ResendDeadLetter>()
            .AddCommand<DeleteDeadLetter>()
            .AddCommand<CancelScheduled>()
            .UseTypeActivator(commandTypes =>
            {
                var services = new ServiceCollection();

                services.AddSingleton<ServiceBus>();

                foreach (var commandType in commandTypes)
                    services.AddTransient(commandType);

                return services.BuildServiceProvider();
            })
            .SetExecutableName("azmsgctl")
            .Build()
            .RunAsync();