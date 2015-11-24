using System;
using CodeComb.Package;

namespace PipeCI.TaskDispatcher.Abstractions
{
    public class Output
    {
        public string TaskId { get; set; }

        public OutputType Type { get; set; }

        public OSType OS { get; set; }

        public string Text { get; set; }

        public DateTime Time { get; set; }
    }
}
