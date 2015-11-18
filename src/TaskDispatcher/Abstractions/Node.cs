using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.Package;

namespace PipeCI.TaskDispatcher.Abstractions
{
    public abstract class Node
    {
        #region Properties
        public string Alias { get; set; }

        public string PrivateKey { get; set; }

        public int Ping { get; set; }

        public int CurrentTaskCount { get; set; }

        public int QueuedTaskCount { get; set; }

        public OSType OS { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public ulong LostConnectionCount { get; set; } = 1;

        public ulong ErrorCount { get; set; }
        #endregion

        #region Methods
        public abstract bool HeartBeat();

        public abstract void UpdateNodeInfo();

        public abstract bool SendTask(CITask task);

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
