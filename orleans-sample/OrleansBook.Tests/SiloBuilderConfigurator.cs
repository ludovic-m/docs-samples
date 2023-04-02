using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Orleans.Runtime;
using Orleans.TestingHost;
using OrleansBook.GrainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.Tests
{
    public class SiloBuilderConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder.AddMemoryGrainStorage("robotStateStore");

            var mockState = new Mock<IPersistentState<RobotState>>();
            mockState.SetupGet(s => s.State).Returns(new RobotState());

            siloBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IPersistentState<RobotState>>(mockState.Object);
                services.AddSingleton<ILogger<RobotGrain>>(new Mock<ILogger<RobotGrain>>().Object);
            });
        }
    }
}
