using System.Threading.Tasks;

namespace PipeCI.TaskDispatcher.CenterSide
{
    public class CITask : Abstractions.CITask
    {
        public async Task<bool> SendToNode(Node node)
        {
            return await node.SendTaskAsync(this);
        }
    }
}
