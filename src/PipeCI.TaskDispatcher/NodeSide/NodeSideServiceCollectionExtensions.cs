using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipeCI.TaskDispatcher.NodeSide;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NodeSideServiceCollectionExtensions
    {
        public static IServiceCollection AddNodeManager(this IServiceCollection self)
        {
            return self.AddSingleton<NodeManager>();
        }
    }
}
