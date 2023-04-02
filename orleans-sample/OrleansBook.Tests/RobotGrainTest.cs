using Orleans.TestingHost;
using OrleansBook.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.Tests
{
    [TestClass]
    public class RobotGrainTest
    {
        static TestCluster cluster;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            cluster = new TestClusterBuilder()
                .AddSiloBuilderConfigurator<SiloBuilderConfigurator>()
                .Build();
            cluster.Deploy();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            cluster.StopAllSilos();
        }

        [TestMethod]
        public async Task TestInstructions()
        {
            var robot = cluster.GrainFactory.GetGrain<IRobotGrain>("test");

            await robot.AddInstruction("move");
            string instruction = await robot.GetNextInstruction();

            Assert.AreEqual("test", instruction, "Get Next Instruction doesn't return the only instruction we added");
        }
    }
}
