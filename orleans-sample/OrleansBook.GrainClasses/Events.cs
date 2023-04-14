using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.GrainClasses
{
    public interface IEvent { }

    public class EnqueueEvent : IEvent
    {
        public string Value { get; }
        public EnqueueEvent() { }
        public EnqueueEvent(string value)
        {
            this.Value = value;
        }
    }

    public class DequeueEvent : IEvent
    {
        public string Value { get; set; }
        public DequeueEvent() { }
    }
}
