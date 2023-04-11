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
        // All methods use the Transaction attribute, even though only the AddInstruction will alter the state
        // They all read the state ahd therefore need to participate in the transaction

        //[Transaction(TransactionOption.CreateOrJoin)] // For Transactions
        Task AddInstruction(string instruction);

        //[Transaction(TransactionOption.CreateOrJoin)] // For Transactions
        Task<string> GetNextInstruction();

        //[Transaction(TransactionOption.CreateOrJoin)] // For Transactions
        Task<int> GetInstructionCount();
    }
}
