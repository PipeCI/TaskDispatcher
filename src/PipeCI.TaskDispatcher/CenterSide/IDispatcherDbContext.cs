using Microsoft.Data.Entity;

namespace PipeCI.TaskDispatcher.CenterSide
{
    public interface IDispatcherDbContext
    {
        DbSet<Node> Nodes { get; set; }
        DbSet<CITask> CITasks { get; set; }
        DbSet<Output> Outputs { get; set; }
        int SaveChanges();
        void OnModelCreating(ModelBuilder builder);
    }
}
