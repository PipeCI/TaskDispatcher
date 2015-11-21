using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.Package;

namespace PipeCI.TaskDispatcher
{
    public class CITask
    {
        public CITask()
        {

        }

        public string Id { get; set; }

        public RestoreMethod RestoreMethod { get; set; }

        public string Url { get; set; }

        public byte[] ZipArchive { get; set; }

        public CITaskStatus Status { get; set; }

        public DateTime? Begin { get; set; }

        public DateTime? End { get; set; }

        public string Branch { get; set; }

        public string Dependency { get; set; }
    }
}