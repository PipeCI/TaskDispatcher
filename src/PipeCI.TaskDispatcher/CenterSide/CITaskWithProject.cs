using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipeCI.TaskDispatcher.CenterSide
{
    public class CITaskWithProject : CITask
    {        
        public string ProjectId { get; set; }

        public Project Project { get; set; }

        public virtual new CITaskWithProject DependencyTask { get; set; }

        public virtual new ICollection<CITaskWithProject> BeDependedTasks { get; set; } = new List<CITaskWithProject>();
    }
}
