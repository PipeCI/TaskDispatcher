using System;

namespace PipeCI.TaskDispatcher.Abstractions
{
    public abstract class CITask
    {
        public CITask()
        {

        }

        public virtual string Id { get; set; }

        public virtual RestoreMethod RestoreMethod { get; set; }

        public virtual string Uri { get; set; }

        public virtual byte[] ZipArchive { get; set; }

        public virtual CITaskStatus Status { get; set; }

        public virtual DateTime? Begin { get; set; }

        public virtual DateTime? End { get; set; }

        public virtual string Branch { get; set; }

        public virtual string Dependency { get; set; }

        public virtual string Version { get; set; }
    }
}