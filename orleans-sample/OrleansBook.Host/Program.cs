using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Program
{
    public async static Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var host = new HostBuilder()
            .UseOrleans(siloBuilder =>
            {
                siloBuilder.UseLocalhostClustering();

                siloBuilder.UseDashboard();
                siloBuilder.AddMemoryGrainStorage("robotStateStore");
                siloBuilder.AddMemoryStreams("StreamProvider");
                siloBuilder.AddMemoryGrainStorage("PubSubStore");
            })
            .ConfigureLogging(x => {
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