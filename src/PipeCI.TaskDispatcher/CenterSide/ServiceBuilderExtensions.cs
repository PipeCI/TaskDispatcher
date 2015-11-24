using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipeCI.TaskDispatcher.CenterSide;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuilderExtensions
    {
        public static IServiceCollection AddTaskDispatcher<TContext>(this IServiceCollection self)
            where TContext : IDispatcherDbContext
        {
            return self;
        }
    }
}
