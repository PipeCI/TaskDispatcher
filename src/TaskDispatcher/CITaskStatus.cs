using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipeCI.TaskDispatcher
{
    public enum CITaskStatus
    {
        Passing,
        Failing,
        Error,
        Building,
        Pending,
        Ignored
    }
}
