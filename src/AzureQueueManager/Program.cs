using CliFx;

await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .SetExecutableName("azmsgctl")
            .Build()
            .RunAsync();