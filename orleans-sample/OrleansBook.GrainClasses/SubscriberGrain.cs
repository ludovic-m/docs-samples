using Orleans.Runtime;
using Orleans.Streams;
using OrleansBook.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.GrainClasses
{
    [ImplicitStreamSubscription("StartingInstruction")]
    public class SubscriberGrain : Grain, ISubscriberGrain, IAsyncObserver<InstructionMessage>
    {
        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var key = this.GetPrimaryKey();
            var streamId = StreamId.Create("StartingInstruction", key);

            await this.GetStreamProvider("StreamProvider")
                .GetStream<InstructionMessage>(streamId)
                .SubscribeAsync(this);
            await base.OnActivateAsync(cancellationToken);
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }

        public Task OnNextAsync(InstructionMessage item, StreamSequenceToken? token = null)
        {
            var msg = $"{item.Robot} starting {item.Instruction}";
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }
    }
}
