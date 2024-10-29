using System;
using Docomposer.Core.Domain;
using Docomposer.Core.Domain.WorkflowConfig;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Docomposer.Core.Test.Domain.WorkflowConfig
{
    [TestFixture]
    public class TestWorkflowConfig
    {
        [Test]
        public void TestEmptyConfiguration()
        {
            var config = new Docomposer.Core.Domain.WorkflowConfig.WorkflowConfig
            {
                Source = new WorkflowStepSource
                {
                    Type = WorkflowSourceType.Documents,
                    Document = new Document
                    {
                        Id = 0,
                        Name = "",
                        ProjectId = 0
                    }
                },
                Generate = new WorkflowStepGenerate
                {
                    
                },
                Parameters = new WorkflowStepParameters
                {
                    Type = WorkflowParametersType.ManualEntry
                },
                SendTo = new WorkflowStepSendTo
                {
                    
                }
            };
            
            Console.WriteLine(config.Configuration());

            var str = config.Configuration();
            
            var sameConfig = JsonConvert.DeserializeObject<Docomposer.Core.Domain.WorkflowConfig.WorkflowConfig>(str);
            
            Assert.That(sameConfig.Source.Type, Is.EqualTo(WorkflowSourceType.Documents));
        }
        
        [Test]
        public void TestConfiguration()
        {
            var config = new Docomposer.Core.Domain.WorkflowConfig.WorkflowConfig
            {
                Source = new WorkflowStepSource
                {
                    Type = WorkflowSourceType.Documents,
                    Document = new Document
                    {
                        Id = 1,
                        Name = "Document 1",
                        ProjectId = 1
                    }
                },
                Generate = null,
                Parameters = new WorkflowStepParameters
                {
                    Type = WorkflowParametersType.ManualEntry
                },
                SendTo = null
            };
            
            Console.WriteLine(config.Configuration());

            var str = config.Configuration();
            
            var sameConfig = JsonConvert.DeserializeObject<Docomposer.Core.Domain.WorkflowConfig.WorkflowConfig>(str);
            
            Assert.That(sameConfig.Source.Type, Is.EqualTo(WorkflowSourceType.Documents));
        }
    }
}