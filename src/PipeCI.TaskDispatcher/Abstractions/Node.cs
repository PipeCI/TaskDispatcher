namespace PipeCI.TaskDispatcher.Abstractions
{
    public abstract class Node : NodeInfo
    {
        #region Properties
        public virtual string Alias { get; set; }

        public virtual string Address { get; set; }

        public virtual int Port { get; set; }

        public virtual string PrivateKey { get; set; }

        public virtual ulong ErrorCount { get; set; }
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
