using System.ComponentModel.DataAnnotations.Schema;

namespace PipeCI.TaskDispatcher.CenterSide
{
    public class Output : Abstractions.Output
    {
        [ForeignKey("TaskId")]
        public virtual CITask CITask { get; set; }
    }
}
