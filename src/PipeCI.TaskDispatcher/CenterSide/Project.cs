using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PipeCI.TaskDispatcher.Abstractions;

namespace PipeCI.TaskDispatcher.CenterSide
{
    public class Project
    {
        [MaxLength(128)]
        public string Id { get; set; }

        public RestoreMethod RestoreMethod { get; set; }

        [MaxLength(512)]
        public string Uri { get; set; }

        [MaxLength(64)]
        public string Branch { get; set; }

        [MaxLength(128)]
        public string Dependency { get; set; }

        public virtual Project DependencyProject { get; set; }

        [MaxLength(64)]
        public string VersionRule { get; set; }

        public ulong CurrentVersion { get; set; }

        [NotMapped]
        public string Version { get { return string.Format(VersionRule, CurrentVersion); } }

        [MaxLength(64)]
        public string YmlHash { get; set; }

        public virtual ICollection<CITaskWithProject> Tasks { get; set; } = new List<CITaskWithProject>();
    }
}
