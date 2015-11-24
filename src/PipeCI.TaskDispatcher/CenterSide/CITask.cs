using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipeCI.TaskDispatcher.CenterSide
{
    public class CITask : Abstractions.CITask
    {
        public bool Linux { get; set; }

        public bool Windows { get; set; }

        public bool OSX { get; set; }

        public string ProjectId { get; set; }

        public Project Project { get; set; }

        public string ShortId { get { return Id.Substring(0, 8); } }

        public DateTime Time { get; set; }

        public virtual CITask DependencyTask { get; set; }

        public virtual ICollection<CITask> BeDependedTasks { get; set; } = new List<CITask>();

        public async Task<bool> SendToNode(Node node)
        {
            if (node == null)
                return false;
            return await node.SendTaskAsync(this);
        }
    }
}
