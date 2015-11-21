using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.Package;

namespace PipeCI.TaskDispatcher.Abstractions
{
    public abstract class Node : NodeInfo
    {
        #region Properties
        public string Alias { get; set; }

        public string Address { get; set; }

        public string Port { get; set; }

        public string PrivateKey { get; set; }

        public ulong ErrorCount { get; set; }
        #endregion

        #region Methods
        public abstract bool IsInQueue(string id);

        public abstract bool IsInBuilding(string id);

        public abstract bool Abort(string id);

        public virtual bool IsInNode(string id)
        {
            return IsInQueue(id) || IsInBuilding(id);
        }
        #endregion
    }
}
