using System;
using Docomposer.Data.Util;
using LinqToDB;
using System.Linq;
using Docomposer.Core.Domain;
using Docomposer.Core.Domain.WorkflowConfig;
using Docomposer.Data.Databases;
using Docomposer.Data.Databases.DataStore;

namespace Docomposer.Core.Api
{
    public static class Workflows
    {
        public static Workflow GetWorkflowById(int id)
        {
            using (var db = new DocReuseDataConnection())
            {
                var workflow = db.Workflow.FirstOrDefault(t => t.Id == id);

                if (workflow != null)
                {
                    return new Workflow
                    {
                        Id = workflow.Id,
                        Configuration = workflow.Configuration,
                        Name = workflow.Name,
                        ProjectId = workflow.ProjectId
                    };
                }
            }
            
            throw new ArgumentException($"Workflow not found in database");
        }

        public static int UpdateWorkflow(Workflow workflow)
        {
            using var db = new DocReuseDataConnection();

            var storedWorkflow = (from t in db.Workflow
                where t.Id == workflow.Id
                select t).FirstOrDefault();

            if (storedWorkflow != null)
            {
                return db.Workflow.Where(t => t.Id == workflow.Id).Update(t => new Workflow
                {
                    Configuration = workflow.Configuration,
                    Name = workflow.Name,
                    ProjectId = workflow.ProjectId
                });
            }
            
            throw new ArgumentException($"Workflow name {workflow.Name} already in database");
        }

        public static int DeleteWorkflowById(int id)
        {
            using (var db = new DocReuseDataConnection())
            {
                var workflow = db.Workflow.FirstOrDefault(t => t.Id == id);

                if (workflow != null)
                {
                    return db.Project.Where(c => c.Id == id).Delete();
                }
                
                throw new ArgumentException($"Workflow with id {id} not found");
            }
        }

        public static int AddWorkflow(Workflow workflow)
        {
            using (var db = new DocReuseDataConnection())
            {
                var workflows =  (from t in db.Workflow
                    where t.ProjectId == workflow.ProjectId
                    select t).ToList();

                if (!workflows.Exists(t => t.Name.Trim() == workflow.Name.Trim()))
                {
                    var config = new WorkflowConfig
                    {
                        Source = new WorkflowStepSource
                        {
                            Type = WorkflowSourceType.Documents,
                            Document = new Document
                            {
                                Id = 0,
                                ProjectId = workflow.ProjectId
                            },
                            Composition = new Composition
                            {
                                Id = 0,
                                ProjectId = workflow.ProjectId
                            }
                        },
                        Parameters = new WorkflowStepParameters
                        {
                            Type = WorkflowParametersType.ManualEntry
                        },
                        Generate = new WorkflowStepGenerate
                        {
                            Type = WorkflowGenerationType.WordDocument
                        },
                        SendTo = new WorkflowStepSendTo
                        {
                            Type = WorkflowSendToType.Screen
                        }
                    };
                    workflow.Configuration = config.Configuration();
                    return db.InsertWithInt32Identity(workflow);
                }
                
                throw new DatabaseException($"Workflow {workflow.Name} already in database");
            }
        }
    }
}