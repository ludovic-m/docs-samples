using Orleans.Concurrency;
using OrleansBook.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.GrainClasses
{
    [StatelessWorker]
    public class BatchGrain : Grain, IBatchGrain
    {

        public Task AddInstructions((string, string)[] values)
        {
            var tasks = values.Select(v =>
            this.GrainFactory
                .GetGrain<IRobotGrain>(v.Item1)
                .AddInstruction(v.Item2));

            return Task.WhenAll(tasks);
        }
    }
}
