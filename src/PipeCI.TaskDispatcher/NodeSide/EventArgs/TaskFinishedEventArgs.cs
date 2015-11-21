using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipeCI.TaskDispatcher.NodeSide.EventArgs
{
    public class TaskFinishedEventArgs : System.EventArgs
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
    }
}
