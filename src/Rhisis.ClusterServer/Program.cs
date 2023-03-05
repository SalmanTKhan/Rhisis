﻿using LiteNetwork;
using LiteNetwork.Client.Hosting;
using LiteNetwork.Hosting;
using LiteNetwork.Server.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rhisis.ClusterServer.Caching;
using Rhisis.Core.Configuration;
using Rhisis.Core.Extensions;
using Rhisis.Infrastructure.Persistance;
using Rhisis.Protocol;
using System;
using System.Threading.Tasks;

namespace Rhisis.ClusterServer;

internal class Program
{
    public static async Task Main()
    {
        const string culture = "en-US";

        Console.Title = "Rhisis - Cluster Server";
        var host = new HostBuilder()
           .ConfigureAppConfiguration((_, config) =>
           {
               config.AddEnvironmentVariables();
               config.SetBasePath(EnvironmentExtension.GetCurrentEnvironementDirectory());
               config.AddYamlFile("config/cluster-server.yml", optional: false, reloadOnChange: true);
           })
           .ConfigureServices((hostContext, services) =>
           {
               services.AddOptions();
               services.Configure<ClusterServerOptions>(hostContext.Configuration.GetSection("server"));
               services.Configure<ClusterCacheOptions>(hostContext.Configuration.GetSection("cluster-cache-server"));

               services.AddAccountPersistance(hostContext.Configuration.GetSection("account-database").Get<DatabaseOptions>());
               services.AddGamePersistance(hostContext.Configuration.GetSection("game-database").Get<DatabaseOptions>());
           })
           .ConfigureLogging(builder =>
           {
               builder.SetMinimumLevel(LogLevel.Trace);
               builder.AddFilter("LiteNetwork.*", LogLevel.Warning);
               builder.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.Warning);
               builder.AddConsole();
           })
           .ConfigureLiteNetwork((context, builder) =>
           {
               builder.AddLiteServer<ClusterServer>(options =>
               {
                   var serverOptions = context.Configuration.GetSection("server").Get<ClusterServerOptions>();

                   if (serverOptions is null)
                   {
                       throw new InvalidProgramException($"Failed to load login server settings.");
                   }

                   options.Host = serverOptions.Ip;
                   options.Port = serverOptions.Port;
                   options.PacketProcessor = new FlyffPacketProcessor();
                   options.ReceiveStrategy = ReceiveStrategyType.Queued;
               });
               //builder.AddLiteServer<WorldChannelCacheServer>(options =>
               //{

               //});
               builder.AddLiteClient<CoreCacheClient>(options =>
               {
                   var cacheClientOptions = context.Configuration.GetSection("cache").Get<ClusterCacheOptions>();

                   if (cacheClientOptions is null)
                   {
                       throw new InvalidProgramException("Failed to load cluster cache client settings.");
                   }

                   options.Host = cacheClientOptions.Ip;
                   options.Port = cacheClientOptions.Port;
                   options.ReceiveStrategy = ReceiveStrategyType.Queued;
               });
           })
           .UseConsoleLifetime()
           .SetConsoleCulture(culture)
           .Build();

        await host.RunAsync();
    }
}
