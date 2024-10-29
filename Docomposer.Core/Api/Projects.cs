using System;
using System.IO;
using System.Linq;
using Docomposer.Core.Domain;
using Docomposer.Core.Util;
using Docomposer.Data.Databases;
using Docomposer.Data.Databases.DataStore;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Util;
using Docomposer.Utils;
using LinqToDB;

namespace Docomposer.Core.Api
{
    public static class Projects
    {
        public static int AddProject(string title, int? parentId = null)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var projects = (from c in db.Project
                    where c.Id > 0
                    select c).ToList();

                if (!projects.Exists(c => c.Name.ToLower().Trim() == title.ToLower().Trim()))
                {
                    var projectId = db.InsertWithInt32Identity(new TableProject
                    {
                        Name = title,
                        ParentId = parentId
                    });

                    Directory.CreateDirectory(Path.Combine(ThisApp.DocReuseDocumentsPath(), projectId.ToString()));
                    db.CommitTransaction();
                    return projectId;
                }
                throw new ArgumentException($"Project {title} already exists");
            }
        }

        public static void DeleteProject(int id)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                
                var sections = (from s in db.Section
                    where s.Id > 0
                    select s).ToList();
                if (sections.Exists(s => s.ProjectId == id))
                {
                    throw new ArgumentException($"Project is not empty. Sections are present.");
                }

                var documents = (from d in db.Document
                    where d.Id > 0
                    select d).ToList();
                if (documents.Exists(t => t.ProjectId == id))
                {
                    throw new ArgumentException("Project is not empty. Documents are present.");
                }

                db.Project.Where(c => c.Id == id).Delete();
                db.CommitTransaction();
            }
        }

        public static void UpdateProject(Project project)
        {
            using (var db = new DocReuseDataConnection())
            {
                db.BeginTransaction();
                var tableProject = new TableProject
                {
                    Id = project.Id,
                    Name = project.Name,
                    ParentId = project.ParentId
                };
                db.Update(tableProject);
                db.CommitTransaction();
            }
        }
    }
}