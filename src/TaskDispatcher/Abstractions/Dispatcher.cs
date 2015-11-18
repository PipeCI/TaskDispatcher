using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipeCI.TaskDispatcher.Abstractions
{
    public abstract class Dispatcher
    {
        public IList<Node> Nodes { get; set; }

        public abstract void ReloadNodes();

        public abstract bool SendTask(CITask task);
    }
}
