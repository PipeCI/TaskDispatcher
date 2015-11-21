using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.Package;

namespace PipeCI.TaskDispatcher.Abstractions
{
    public class NodeInfo
    {
        public virtual int CurrentTaskCount { get; set; }

        public virtual int QueuedTaskCount { get; set; }

        public virtual OSType OS { get; set; }
    }
}
