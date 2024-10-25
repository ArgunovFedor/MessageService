﻿namespace MessageService.Client.Infrastructure;

public static class Helpers
{
    public static void Configure(this IConfigurationBuilder config,
        HostBuilderContext context,
        string[] args)
    {
        var environmentName = context.HostingEnvironment.EnvironmentName;
        config.AddJsonFile("appsettings.json", true);
        config.AddJsonFile($"appsettings.{environmentName}.json",
            true);
        config.AddJsonFile("appsettings.local.json", true, true);
        config.AddEnvironmentVariables("APP_");
        if (args != null)
        {
            config.AddCommandLine(args);
        }
        config.Build();
    }
}