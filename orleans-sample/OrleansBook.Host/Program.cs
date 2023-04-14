using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Serialization;
using System.Text.Json;

internal class Program
{
    public async static Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json") // Remember to set properties on the appsettings file : Buid Action: Content - Copy to output directory : Copy if newer
        .AddUserSecrets("6541a985-761d-416e-9492-d3ca4fe49e8b")
        .AddEnvironmentVariables()
        .Build();

        var host = new HostBuilder()
            .UseOrleans(siloBuilder =>
            {
                siloBuilder.UseLocalhostClustering();

                //siloBuilder.UseDashboard();
                siloBuilder.AddAzureTableGrainStorage(
                    name: "robotStateStore",
                    configureOptions: options =>
                    {
                        // Configure the storage connection key
                        options.ConfigureTableServiceClient(config["ORLEANS_STORAGE_CONNECTION_STRING"]);
                    });
                siloBuilder.AddAzureTableGrainStorage(
                    name: "PubSubStore",
                    configureOptions: options =>
                    {
                        // Configure the storage connection key
                        options.ConfigureTableServiceClient(config["ORLEANS_STORAGE_CONNECTION_STRING"]);
                    });
                siloBuilder.AddEventHubStreams("StreamProvider", (ISiloEventHubStreamConfigurator configure) =>
                {
                    configure.ConfigureEventHub(builder => builder.Configure(options =>
                    {
                        options.ConfigureEventHubConnection(config["ORLEANS_EVENT_HUB_CONNECTION_STRING"].ToString(), "orleans-stream", "robots");
                    }));
                    configure.UseAzureTableCheckpointer(
                        builder => builder.Configure(options =>
                        {
                            options.ConfigureTableServiceClient(config["ORLEANS_STORAGE_CONNECTION_STRING"]);
                            options.PersistInterval = TimeSpan.FromSeconds(10);
                        }));
                });
                //siloBuilder.UseInMemoryReminderService();

                //if (config["ORLEANS_STORAGE_CONNECTION_STRING"] is { } connectionString)
                //{
                //    siloBuilder.AddAzureTableTransactionalStateStorage(
                //        "TransactionStore",
                //        options => options.ConfigureTableServiceClient(connectionString));
                //}
                //else
                //{
                //    siloBuilder.AddMemoryGrainStorageAsDefault();
                //}
                //siloBuilder.UseTransactions();
            })
            .ConfigureLogging(x =>
            {
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Warning);
            })
            .Build();


        await host.StartAsync();
        Console.WriteLine("Press enter to stop the Silo...");
        Console.ReadLine();

        await host.StopAsync();
    }
}