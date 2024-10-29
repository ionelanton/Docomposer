using System.Linq;
using LinqToDB;
using Newtonsoft.Json.Linq;

namespace Docomposer.Data.Databases.DataStore.Util
{
    public static class MenuBuilder
    {
        // https://www.newtonsoft.com/json/help/html/SerializeWithLinq.htm

        private static JArray BuildFromProjects(string onClick)
        {
            using var db = new DocReuseDataConnection();
            var jProjects = new JArray
            {
                from c in db.Project
                select new JObject
                {
                    {"id", "c-" + c.Id},
                    {"parent", c.ParentId == null ? "c-0" : "c-" + c.ParentId },
                    {"text", c.Name},
                    {"type", "folder"},
                    {"a_attr", new JObject
                        {
                            {"onclick", onClick}
                        }
                    },
                    {
                        "state", new JObject
                        {
                            {"opened", false},
                            {"disabled", false},
                            {"selected", false}
                        }
                    }
                }
            };

            return jProjects;
        }

        public static JArray BuildFromTableSection()
        {
            var jMenuData = BuildFromProjects("docomposer.api.doc.hide();");
            var root = new JObject
            {
                {"id", "c-0"},
                {"parent", "#"},
                {"text", "Sections"},
                {"type", "root"},
                {"a_attr", new JObject
                    {
                        {"onclick", "docomposer.api.doc.hide();"}
                    }
                },
                {
                    "state", new JObject
                    {
                        {"opened", true},
                        {"disabled", false},
                        {"selected", true}
                    }
                }
            };
            jMenuData.Insert(0, root);

            using var db = new DocReuseDataConnection();
            var jSections = new JArray
            {
                from s in db.Section
                from m in db.Project.LeftJoin(m => m.Id == s.ProjectId)
                select new JObject
                {
                    {"id", "s-" + s.Id},
                    {"parent", "c-" + s.ProjectId},
                    {"text", s.Name},
                    {"a_attr", new JObject
                        {
                            {"onclick", "docomposer.api.doc.preview(\"Sections\", " + s.Id + ");"},
                            {"ondblclick", "docomposer.api.doc.open(" + s.ProjectId + ", \"Sections\", \"" + s.Name + "\");"}
                        }
                    },
                    {"type", s == null ? "folder" : "file"},
                    {
                        "state", new JObject
                        {
                            {"disabled", false},
                            {"selected", false}
                        }
                    }
                }
            };

            foreach (var jSection in jSections)
            {
                jMenuData.Add(jSection);
            }

            return jMenuData;
        }
        
        public static JArray BuildFromTableDocument()
        {
            var jMenuData = BuildFromProjects("docomposer.api.doc.hide();");
            var root = new JObject
            {
                {"id", "c-0"},
                {"parent", "#"},
                {"text", "Documents"},
                {"type", "root"},
                {"a_attr", new JObject
                    {
                        {"onclick", "docomposer.api.doc.hide();"}
                    }
                },
                {
                    "state", new JObject
                    {
                        {"opened", true},
                        {"disabled", false},
                        {"selected", true}
                    }
                }
            };
            jMenuData.Insert(0, root);

            using var db = new DocReuseDataConnection();
            var jSections = new JArray
            {
                from d in db.Document
                from m in db.Project.LeftJoin(m => m.Id == d.ProjectId)
                select new JObject
                {
                    {"id", "d-" + d.Id},
                    {"parent", "c-" + d.ProjectId},
                    {"text", d.Name},
                    {"a_attr", new JObject
                        {
                            {"onclick", "docomposer.api.doc.preview(\"Documents\", " + d.Id + ");"},
                            {"ondblclick", "docomposer.api.doc.open(" + d.ProjectId + ", \"Documents\", \"" + d.Name + "\");"}
                        }
                    },
                    {"type", d == null ? "folder" : "file"},
                    {
                        "state", new JObject
                        {
                            {"disabled", false},
                            {"selected", false}
                        }
                    }
                }
            };

            foreach (var jSection in jSections)
            {
                jMenuData.Add(jSection);
            }

            return jMenuData;
        }
        
        public static JArray BuildFromTableCompositions()
        {
            var jMenuData = BuildFromProjects("docomposer.api.doc.hide();");
            var root = new JObject
            {
                {"id", "c-0"},
                {"parent", "#"},
                {"text", "Compositions"},
                {"type", "root"},
                {"a_attr", new JObject
                    {
                        {"onclick", "docomposer.api.doc.hide();"}
                    }
                },
                {
                    "state", new JObject
                    {
                        {"opened", true},
                        {"disabled", false},
                        {"selected", true}
                    }
                }
            };
            jMenuData.Insert(0, root);

            using var db = new DocReuseDataConnection();
            var jCompositions = new JArray
            {
                from d in db.Composition
                from m in db.Project.LeftJoin(m => m.Id == d.ProjectId)
                select new JObject
                {
                    {"id", "o-" + d.Id},
                    {"parent", "c-" + d.ProjectId},
                    {"text", d.Name},
                    {"a_attr", new JObject
                        {
                            {"onclick", "docomposer.api.doc.preview(\"Compositions\", " + d.Id + ");"},
                        }
                    },
                    {"type", d == null ? "folder" : "file"},
                    {
                        "state", new JObject
                        {
                            {"disabled", false},
                            {"selected", false}
                        }
                    }
                }
            };

            foreach (var jComposition in jCompositions)
            {
                jMenuData.Add(jComposition);
            }

            return jMenuData;
        }
        
        public static JArray BuildFromTableWorkflows()
        {
            var jMenuData = BuildFromProjects("docomposer.api.workflow.hide();");
            var root = new JObject
            {
                {"id", "c-0"},
                {"parent", "#"},
                {"text", "Workflows"},
                {"type", "root"},
                {"a_attr", new JObject
                    {
                        {"onclick", "docomposer.api.workflow.hide();"}
                    }
                },
                {
                    "state", new JObject
                    {
                        {"opened", true},
                        {"disabled", false},
                        {"selected", true}
                    }
                }
            };
            jMenuData.Insert(0, root);

            using var db = new DocReuseDataConnection();
            var jCompositions = new JArray
            {
                from p in db.Workflow
                from m in db.Project.LeftJoin(m => m.Id == p.ProjectId)
                select new JObject
                {
                    {"id", "p-" + p.Id},
                    {"parent", "c-" + p.ProjectId},
                    {"text", p.Name},
                    {"a_attr", new JObject
                        {
                            {"onclick", "docomposer.api.workflow.get(" + p.Id + ");"}
                        }
                    },
                    {"type", p == null ? "folder" : "file"},
                    {
                        "state", new JObject
                        {
                            {"disabled", false},
                            {"selected", false}
                        }
                    }
                }
            };

            foreach (var jComposition in jCompositions)
            {
                jMenuData.Add(jComposition);
            }

            return jMenuData;
        }

        public static JArray BuildFromTableDataSources()
        {
            var jMenuData = BuildFromProjects("docomposer.api.datasource.hide();");
            var root = new JObject
            {
                {"id", "c-0"},
                {"parent", "#"},
                {"text", "Data sources"},
                {"type", "root"},
                {"a_attr", new JObject
                    {
                        {"onclick", "docomposer.api.datasource.hide();"}
                    }
                },
                {
                    "state", new JObject
                    {
                        {"opened", true},
                        {"disabled", false},
                        {"selected", true}
                    }
                }
            };
            jMenuData.Insert(0, root);

            using var db = new DocReuseDataConnection();
            var jDataSources = new JArray
            {
                from d in db.DataSource
                from m in db.Project.LeftJoin(m => m.Id == d.ProjectId)
                select new JObject
                {
                    {"id", "d-" + d.Id},
                    {"parent", "c-" + d.ProjectId},
                    {"text", d.Name},
                    {"a_attr", new JObject
                        {
                            {"onclick", "docomposer.api.datasource.get(" + d.Id + ");" },
                        }
                    },
                    {"type", d == null ? "folder" : "file"},
                    {
                        "state", new JObject
                        {
                            {"disabled", false},
                            {"selected", false}
                        }
                    }
                }
            };

            foreach (var jDataSource in jDataSources)
            {
                jMenuData.Add(jDataSource);
            }

            return jMenuData;
        }
    }
}