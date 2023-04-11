﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.GrainInterfaces
{
    public  interface IBatchGrain : IGrainWithIntegerKey
    {
        Task AddInstructions((string, string)[] values);
    }
}
