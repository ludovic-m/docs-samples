using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Transactions.Abstractions;
using OrleansBook.GrainInterfaces;
using OrleansCodeGen.Orleans;
using OrleansCodeGen.Orleans.Transactions.Abstractions;

namespace OrleansBook.GrainClasses
{

    public class RobotGrain : Grain, IRobotGrain, IRemindable
    {
        //private Queue<string> instructions = new();

        private int instructionsEnqueud = 0;
        private int instructionsDequeud = 0;
        private readonly ILogger<RobotGrain> _logger;
        private readonly IPersistentState<RobotState> _state;
        private readonly IAsyncStream<InstructionMessage> stream;

        public RobotGrain(ILogger<RobotGrain> logger, 
            [PersistentState("robotState", "robotStateStore")] IPersistentState<RobotState> state) 
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

            // Without Transaction

            this._state.State.Instructions.Enqueue(instruction);
            this.instructionsEnqueud += 1;
            // this.instructions.Enqueue(instruction);
            await this._state.WriteStateAsync();

            // With Transaction
            //await this._transactionState.PerformUpdate(state => state.Instructions.Enqueue(instruction));
        }

        public async Task<int> GetInstructionCount()
        {
            // Without Transaction 
            return await Task.FromResult(this._state.State.Instructions.Count);

            // With Transaction
            //return await this._transactionState.PerformRead(r => r.Instructions.Count);
        }

        /// <summary>
        /// This methode is not thread safe without Orleans (someone dequeue the last instruction after another thread has evaluated the count and before it dequeues it).
        /// Orleans use "turn-based concurrency" to ensure that it won't happen.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetNextInstruction()
        {
            var key = this.GetPrimaryKeyString();

            // Without Transaction

            if (this._state.State.Instructions.Count == 0)
            {
                return null;
            }

            var instruction = this._state.State.Instructions.Dequeue();



            await this.Publish(instruction, key);
            this.instructionsDequeud += 1;
            await this._state.WriteStateAsync();

            // With Transaction

            //string instruction = null;
            //await this._transactionState.PerformUpdate(s =>
            //{
            //    if (s.Instructions.Count == 0)
            //        return;

            //    instruction = s.Instructions.Dequeue();
            //});

            if (instruction is not null)
                this._logger.LogWarning($"{key} executing {instruction}");

            return instruction;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            //this.RegisterTimer(this.Reset, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            //await this.RegisterOrUpdateReminder("firmware", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            await base.OnActivateAsync(cancellationToken);
        }

        Task Reset(object _)
        {
            var key = this.GetPrimaryKeyString();

            Console.WriteLine($"{key} enqueued: {this.instructionsEnqueud}");
            Console.WriteLine($"{key} dequeued: {this.instructionsDequeud}");
            Console.WriteLine($"{key} queued: {this._state.State.Instructions.Count}");

            this.instructionsDequeud = 0;
            this.instructionsEnqueud = 0;

            return Task.CompletedTask;
        }

        public Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if(reminderName == "firmware")
            {
                return this.AddInstruction("Update Firmware");
            }
            return Task.CompletedTask;
        }
    }
}
