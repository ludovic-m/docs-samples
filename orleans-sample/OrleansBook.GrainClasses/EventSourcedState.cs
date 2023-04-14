using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.GrainClasses
{
    public class EventSourcedState
    {
        Queue<string> Instructions = new Queue<string>();

        public int Count => this.Instructions.Count;

        public void Apply(EnqueueEvent @event) =>
            this.Instructions.Enqueue(@event.Value);

        public void Apply(DequeueEvent @event)
        {
            if (this.Instructions.Count == 0) return;
            @event.Value = this.Instructions.Dequeue();
        }
    }
}
