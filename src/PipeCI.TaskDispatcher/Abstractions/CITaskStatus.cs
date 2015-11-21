namespace PipeCI.TaskDispatcher.Abstractions
{
    public enum CITaskStatus
    {
        Passing,
        Failing,
        Error,
        Building,
        Pending,
        Ignored
    }
}
