using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.Package;
using PipeCI.TaskDispatcher.Abstractions;

namespace PipeCI.TaskDispatcher.CenterSide
{
    public class Dispatcher<TContext> : Dispatcher<CITask, Node>
        where TContext : IDispatcherDbContext
    {
        public Dispatcher(TContext dbContext)
        {
            DbContext = dbContext;
        }

        public IDispatcherDbContext DbContext { get; private set; }

        public override IList<Node> Nodes
        {
            get
            {
                return DbContext.Nodes
                    .Where(x => x.Enabled)
                    .ToList();
            }
        }

        public override string GenerateIdentifier(Abstractions.CITask task)
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public override Node GetFreeNode(OSType os)
        {
            return Nodes.Where(x => x.OS == os)
                .OrderBy(x => x.CurrentTaskCount + x.QueuedTaskCount)
                .FirstOrDefault();
        }

        public override void Output(string id, string text, OutputType type, OSType os , DateTime time)
        {
            var task = DbContext.CITasks.Where(x => x.Id == id).FirstOrDefault();
            if (task != null)
            {
                DbContext.Outputs.Add(new Output
                {
                    TaskId = id,
                    Text = text,
                    Time = time,
                    Type = type,
                    OS = os
                });
                DbContext.SaveChanges();
            }
        }

        public override async Task<bool> SendTaskAsync(CITask task)
        {
            var flag = true;
            if (task.ProjectId == null || task.Project.Windows)
            {
                flag = await task.SendToNode(GetFreeNode(OSType.Windows));
            }
            if (task.ProjectId == null || task.Project.Linux)
            {
                flag = await task.SendToNode(GetFreeNode(OSType.Linux));
            }
            if (task.ProjectId == null || task.Project.OSX)
            {
                flag = await task.SendToNode(GetFreeNode(OSType.OSX));
            }
            return flag;
        }

        public override void UpdateStatus(string id, CITaskStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
