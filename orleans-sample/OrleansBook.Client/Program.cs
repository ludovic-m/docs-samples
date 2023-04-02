using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.Client
{
    internal class Program
    {
        public async static Task Main(string[] args)
        {
            var hostClient = new HostBuilder()
                .UseOrleansClient((context, clientBuilder) =>
                {
                    clientBuilder.UseLocalhostClustering();
                })
                .ConfigureLogging(x => {
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel.Warning);
                })
                .Build();

            using (hostClient)
            {
                await hostClient.StartAsync();

                while (true)
                {
                    Console.WriteLine("Please enter a robot name...");
                    var grainId = Console.ReadLine();
                    var grainFactory = hostClient.Services.GetRequiredService<IGrainFactory>();

                    var grain = grainFactory.GetGrain<IRobotGrain>(grainId);

                    Console.WriteLine("Please enter an instruction...");
                    var instruction = Console.ReadLine();
                    await grain.AddInstruction(instruction);

                    var count = await grain.GetInstructionCount();
                    Console.WriteLine($"{grainId} has {count} instruction(s)");
                }
            }
        }
    }
}
