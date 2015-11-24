using Microsoft.Data.Entity;

namespace PipeCI.TaskDispatcher.CenterSide
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder SetupDispatcherDbContext(this ModelBuilder self)
        {
            return self.SetupCITasks()
                .SetupNodes()
                .SetuptOutputs()
                .SetupProjects();
        }

        public static ModelBuilder SetupNodes(this ModelBuilder self)
        {
            return self.Entity<Node>(e =>
            {
                e.Ignore(x => x.Ping);
                e.Ignore(x => x.OS);
                e.Ignore(x => x.CurrentTaskCount);
                e.Ignore(x => x.MaxThreadsCount);
                e.Ignore(x => x.LostConnectionCount);
                e.Ignore(x => x.QueuedTaskCount);
                e.Ignore(x => x.ErrorCount);
                e.Property(x => x.Address).HasMaxLength(64);
                e.Property(x => x.Alias).HasMaxLength(64);
                e.Property(x => x.PrivateKey).HasMaxLength(64);
                e.HasIndex(x => x.Enabled);
            });
        }

        public static ModelBuilder SetuptOutputs(this ModelBuilder self)
        {
            return self.Entity<Output>(e =>
            {
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.Type);
                e.HasIndex(x => x.OS);
            });
        }

        public static ModelBuilder SetupCITasks(this ModelBuilder self)
        {
            return self.Entity<CITask>(e =>
            {
                e.Ignore(x => x.ShortId);
                e.Property(x => x.Id).HasMaxLength(128);
                e.Property(x => x.Version).HasMaxLength(128);
                e.Property(x => x.Branch).HasMaxLength(64);
                e.Property(x => x.Dependency).HasMaxLength(128);
                e.HasIndex(x => x.Status);
                e.HasIndex(x => x.Begin);
                e.HasIndex(x => x.End);
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.OSX);
                e.HasIndex(x => x.Linux);
                e.HasIndex(x => x.Windows);
                e.HasOne(x => x.DependencyTask).WithMany(x => x.BeDependedTasks).IsRequired(false).OnDelete(Microsoft.Data.Entity.Metadata.DeleteBehavior.SetNull);
            });
        }

        public static ModelBuilder SetupProjects(this ModelBuilder self)
        {
            return self.Entity<Project>(e =>
            {
                e.HasIndex(x => x.OSX);
                e.HasIndex(x => x.Linux);
                e.HasIndex(x => x.Windows);
                e.HasIndex(x => x.LastYmlHash);
                e.HasMany(x => x.Tasks).WithOne(x => x.Project).IsRequired(false);
            });
        }
    }
}
