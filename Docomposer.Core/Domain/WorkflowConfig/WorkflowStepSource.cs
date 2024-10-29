namespace Docomposer.Core.Domain.WorkflowConfig
{
    public class WorkflowStepSource
    {
        public WorkflowSourceType Type { get; set; }
        public Document Document { get; set; }
        public Composition Composition { get; set; }
    }
}