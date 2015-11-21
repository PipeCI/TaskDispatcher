using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipeCI.TaskDispatcher.Abstractions
{
    public class Output
    {
        public string TaskId { get; set; }

        public OutputType Type { get; set; }

        public string Text { get; set; }

        public DateTime Time { get; set; }
    }
}
