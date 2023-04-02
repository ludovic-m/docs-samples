using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using OrleansBook.GrainInterfaces;
using OrleansCodeGen.Orleans;

namespace OrleansBook.GrainClasses
{

    public class RobotGrain : Grain, IRobotGrain
    {
        //private Queue<string> instructions = new();

        private readonly ILogger<RobotGrain> _logger;
        private readonly IPersistentState<RobotState> _state;
        private readonly IAsyncStream<InstructionMessage> stream;

        public RobotGrain(ILogger<RobotGrain> logger, [PersistentState("robotState", "robotStateStore")] IPersistentState<RobotState> state)
        {
            _logger = logger;
            _state = state;
            stream = this.GetStreamProvider("StreamProvider")
                .GetStream<InstructionMessage>("StartingInstruction", Guid.Empty);
        }

        Task Publish(string instruction, string key)
        {
            var message = new InstructionMessage(instruction, key);
            return this.stream.OnNextAsync(message); // Add message to the stream
        }

        public async Task AddInstruction(string instruction)
        {
            var key = this.GetPrimaryKeyString();
            this._logger.LogWarning($"{key} adding {instruction}");


            this._state.State.Instructions.Enqueue(instruction);
            //this.instructions.Enqueue(instruction);
            await this._state.WriteStateAsync();
        }

        public Task<int> GetInstructionCount()
        {
            return Task.FromResult(this._state.State.Instructions.Count);
        }

        /// <summary>
        /// This methode is not thread safe without Orleans (someone dequeue the last instruction after another thread has evaluated the count and before it dequeues it).
        /// Orleans use "turn-based concurrency" to ensure that it won't happen.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetNextInstruction()
        {
            if(this._state.State.Instructions.Count == 0)
            {
                return null;
            }

            var instruction = this._state.State.Instructions.Dequeue();

            var key = this.GetPrimaryKeyString();
            this._logger.LogWarning($"{key} executing {instruction}");

            await this.Publish(instruction, key);

            await this._state.WriteStateAsync();
            return instruction;
        }
    }
}
