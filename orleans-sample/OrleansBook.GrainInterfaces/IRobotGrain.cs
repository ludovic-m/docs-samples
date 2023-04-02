using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.GrainInterfaces
{
    public interface IRobotGrain : IGrainWithStringKey
    {
        Task AddInstruction(string instruction);
        Task<string> GetNextInstruction();
        Task<int> GetInstructionCount();
    }
}
