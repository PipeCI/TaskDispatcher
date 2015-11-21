using System;

namespace PipeCI.TaskDispatcher.Abstractions
{
    public abstract class CITask
    {
        public CITask()
        {

        }

        public string Id { get; set; }

        public RestoreMethod RestoreMethod { get; set; }

        public string Uri { get; set; }

        public byte[] ZipArchive { get; set; }

        public CITaskStatus Status { get; set; }

        public DateTime? Begin { get; set; }

        public DateTime? End { get; set; }

        public string Branch { get; set; }

        public string Dependency { get; set; }

        public string LastYmlHash { get; set; }

        public string Version { get; set; }
    }
}