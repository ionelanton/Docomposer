using Newtonsoft.Json;

namespace Docomposer.Core.Domain.WorkflowConfig
{
    public class WorkflowConfig : IWorkflowConfig
    {
        public WorkflowStepSource Source { get; set; }
        public WorkflowStepParameters Parameters { get; set; }
        public WorkflowStepGenerate Generate { get; set; }
        public WorkflowStepSendTo SendTo { get; set; }
        
        public string Configuration()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}