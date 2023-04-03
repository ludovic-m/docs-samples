using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.GrainInterfaces
{
    [GenerateSerializer]
    public class InstructionMessage
    {
        [Id(0)]
        public string Instruction { get; set; }
        [Id(1)]
        public string Robot { get; set; }

        public InstructionMessage() { } // Constructor with no arguments so the serializer can instantiate it

        public InstructionMessage(string _instruction, string _robot)
        {
            Instruction = _instruction;
            Robot = _robot;
        }
    }
}
