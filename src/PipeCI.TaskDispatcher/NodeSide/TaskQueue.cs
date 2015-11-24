using System.Collections.Generic;

namespace PipeCI.TaskDispatcher.NodeSide
{
    public class CITaskQueue : List<CITask>
    {
        public CITask Dequeue()
        {
            if (Count == 0)
                return null;
            var ret = this[0];
            this.Remove(ret);
            return ret;
        }

        public void Enqueue(CITask task)
        {
            Add(task);
        }
    }
}
