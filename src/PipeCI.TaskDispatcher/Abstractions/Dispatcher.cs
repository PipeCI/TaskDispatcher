using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeComb.Package;

namespace PipeCI.TaskDispatcher.Abstractions
{
    public abstract class Dispatcher<TTask, TNode>
        where TTask : CITask
        where TNode : Node
    {
        public virtual IList<TTask> TaskQueue { get; protected set; }

        public virtual IList<TTask> TaskBuilding { get; protected set; }

        public virtual IList<TNode> Nodes { get; }

        public abstract string GenerateIdentifier(CITask task);

        public abstract Task<bool> SendTaskAsync(TTask task);

        public abstract void UpdateStatus(string id, CITaskStatus status);

        public abstract void Output(string id, string text, OutputType type, OSType os, DateTime time);

        public abstract TNode GetFreeNode(OSType os);
    }
}
