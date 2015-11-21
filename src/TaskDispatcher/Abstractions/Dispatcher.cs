using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipeCI.TaskDispatcher.Abstractions
{
    public abstract class Dispatcher
    {
        public IList<CITask> TaskQueue { get; protected set; }

        public IList<CITask> TaskBuilding { get; protected set; }

        public IList<Node> Nodes { get; set; }

        public abstract void ReloadNodes();

        public abstract bool SendTask(CITask task);
    }
}
