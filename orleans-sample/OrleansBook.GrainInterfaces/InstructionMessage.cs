using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.GrainInterfaces
{
    public class InstructionMessage
    {
        public string Instruction { get; set; }
        public string Robot { get; set; }

        public InstructionMessage() { } // Constructor with no arguments so the serializer can instantiate it

        public InstructionMessage(string _instruction, string _robot)
        {
            Instruction = _instruction;
            Robot = _robot;
        }
    }
}
