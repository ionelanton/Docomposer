using System.Collections.Generic;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Data.Util;
using LinqToDB;
using Newtonsoft.Json;

namespace Docomposer.Data.Databases.DataStore.Util
{
    public class DatabasePopulator
    {
        private readonly List<int> _projectIds = [];
        private readonly List<int> _sectionIds = [];
        private readonly List<int> _documentIds = [];
        private readonly List<int> _compositionIds = [];
        private readonly List<int> _dataSourcesIds = [];

        public void Populate()
        {
            PopulateTableProject();
            PopulateTableSection();
            PopulateTableDocument();
            PopulateTableSectionDocument();
            PopulateTableComposition();
            PopulateTableDocumentComposition();
            PopulateTableDataSource();
            PopulateTableDataQuery();
            PopulateTableWorkflow();
        }

        private void PopulateTableProject()
        {
            using var db = new DocReuseDataConnection();

            _projectIds.Add(db.InsertWithInt32Identity(new TableProject
            {
                Name = "01. Sample project",
                ParentId = null,
            }));

            _projectIds.Add(db.InsertWithInt32Identity(new TableProject
            {
                Name = "90. Miscellaneous examples",
                ParentId = null,
            }));

            _projectIds.Add(db.InsertWithInt32Identity(new TableProject
            {
                Name = "91. Excel data source examples",
                ParentId = null,
            }));

            _projectIds.Add(db.InsertWithInt32Identity(new TableProject
            {
                Name = "92. CSV data source examples",
                ParentId = null,
            }));

            _projectIds.Add(db.InsertWithInt32Identity(new TableProject
            {
                Name = "93. SQLite data source examples",
                ParentId = null,
            }));
        }

        private void PopulateTableSection()
        {
            using var db = new DocReuseDataConnection();

            _sectionIds.Add(db.InsertWithInt32Identity(new TableSection
                {
                    ProjectId = _projectIds[1],
                    Name = "Cat 1",
                }
            ));

            _sectionIds.Add(db.InsertWithInt32Identity(new TableSection
                {
                    ProjectId = _projectIds[1],
                    Name = "Cat 2",
                }
            ));

            _sectionIds.Add(db.InsertWithInt32Identity(new TableSection
                {
                    ProjectId = _projectIds[1],
                    Name = "Cat 3",
                }
            ));
            
            _sectionIds.Add(db.InsertWithInt32Identity(new TableSection
                {
                    ProjectId = _projectIds[2],
                    Name = "Customer letter header",
                }
            ));
            
            _sectionIds.Add(db.InsertWithInt32Identity(new TableSection
                {
                    ProjectId = _projectIds[2],
                    Name = "Customer letter content",
                }
            ));
            
            _sectionIds.Add(db.InsertWithInt32Identity(new TableSection
                {
                    ProjectId = _projectIds[2],
                    Name = "Customer invoice list",
                }
            ));
            
            _sectionIds.Add(db.InsertWithInt32Identity(new TableSection
            {
                ProjectId = _projectIds[2],
                Name = "Northwind products section"
            }));
        }

        private void PopulateTableDocument()
        {
            using var db = new DocReuseDataConnection();

            _documentIds.Add(db.InsertWithInt32Identity(new TableDocument
            {
                Name = "01. Cat catalog cover",
                ProjectId = _projectIds[1]
            }));

            _documentIds.Add(db.InsertWithInt32Identity(new TableDocument
            {
                Name = "02. Cat catalog content",
                ProjectId = _projectIds[1]
            }));
            
            _documentIds.Add(db.InsertWithInt32Identity(new TableDocument
            {
                Name = "Customer sale promotion",
                ProjectId = _projectIds[2]
            }));
            
            _documentIds.Add(db.InsertWithInt32Identity(new TableDocument
            {
                Name = "Customer invoices",
                ProjectId = _projectIds[2]
            }));
            
            _documentIds.Add(db.InsertWithInt32Identity(new TableDocument
            {
                Name = "Northwind products template",
                ProjectId = _projectIds[2]
            }));
        }

        private void PopulateTableSectionDocument()
        {
            using var db = new DocReuseDataConnection();

            db.InsertWithInt32Identity(new TableSectionDocument
            {
                DocumentId = _documentIds[1],
                SectionId = _sectionIds[0],
                PredecessorId = 0
            });
            db.InsertWithInt32Identity(new TableSectionDocument
            {
                DocumentId = _documentIds[1],
                SectionId = _sectionIds[1],
                PredecessorId = 1
            });
            db.InsertWithInt32Identity(new TableSectionDocument
            {
                DocumentId = _documentIds[1],
                SectionId = _sectionIds[2],
                PredecessorId = 2
            });
            db.InsertWithInt32Identity(new TableSectionDocument
            {
                DocumentId = _documentIds[2],
                SectionId = _sectionIds[3],
                PredecessorId = 0
            });
            db.InsertWithInt32Identity(new TableSectionDocument
            {
                DocumentId = _documentIds[2],
                SectionId = _sectionIds[4],
                PredecessorId = 4
            });
            db.InsertWithInt32Identity(new TableSectionDocument
            {
                DocumentId = _documentIds[3],
                SectionId = _sectionIds[5],
                PredecessorId = 0
            });
            db.InsertWithInt32Identity(new TableSectionDocument
            {
                DocumentId = _documentIds[4],
                SectionId = _sectionIds[6],
                PredecessorId = 0
            });
        }

        private void PopulateTableComposition()
        {
            using var db = new DocReuseDataConnection();

            _compositionIds.Add(db.InsertWithInt32Identity(new TableComposition
            {
                Name = "Cat catalog",
                ProjectId = _projectIds[1]
            }));
            
            _compositionIds.Add(db.InsertWithInt32Identity(new TableComposition
            {
                Name = "Customer promotional letter",
                ProjectId = _projectIds[2]
            }));
        }

        private void PopulateTableDocumentComposition()
        {
            using var db = new DocReuseDataConnection();

            db.InsertWithInt32Identity(new TableDocumentComposition
            {
                CompositionId = _compositionIds[0],
                DocumentId = _documentIds[0],
                PredecessorId = 0
            });
            db.InsertWithInt32Identity(new TableDocumentComposition
            {
                CompositionId = _compositionIds[0],
                DocumentId = _documentIds[1],
                PredecessorId = 1
            });
            
            db.InsertWithInt32Identity(new TableDocumentComposition
            {
                CompositionId = _compositionIds[1],
                DocumentId = _documentIds[2],
                PredecessorId = 0
            });
            db.InsertWithInt32Identity(new TableDocumentComposition
            {
                CompositionId = _compositionIds[1],
                DocumentId = _documentIds[3],
                PredecessorId = 3
            });
        }

        private void PopulateTableDataSource()
        {
            using var db = new DocReuseDataConnection();

            _dataSourcesIds.Add(db.InsertWithInt32Identity(new TableDataSource
            {
                ProjectId = _projectIds[2],
                Name = "Chinook",
                Configuration = @"{""fileName"":""Chinook.xlsx""}",
                Type = DataSourceEnum.Excel
            }));
            _dataSourcesIds.Add(db.InsertWithInt32Identity(new TableDataSource
            {
                ProjectId = _projectIds[2],
                Name = "Northwind",
                Configuration = @"{""fileName"":""Northwind.xlsx""}",
                Type = DataSourceEnum.Excel
            }));
        }

        private void PopulateTableDataQuery()
        {
            using var db = new DocReuseDataConnection();

            db.Insert(new TableDataQuery
            {
                Name = "Customer",
                Description = "Chinook customer by Id",
                DataSourceId = _dataSourcesIds[0],
                Statement = "SELECT CustomerId, FirstName, LastName, Address,\n       City, State, PostalCode, Phone, Email\nFROM customers\nWHERE CustomerId = @CustomerId;",
                Parameters = JsonConvert.SerializeObject(new List<SqlParam>
                {
                    new()
                    {
                        Name = "CustomerId",
                        Value = "2"
                    }
                })
                
            });
            
            db.Insert(new TableDataQuery
            {
                Name = "invoices",
                Description = "List of invoices by customer Id",
                DataSourceId = _dataSourcesIds[0],
                Statement = "SELECT InvoiceId AS Id, DATE((InvoiceDate * 3600 * 24) - 2209161600, 'unixepoch') AS Date, Total AS Amount\nFROM invoices\nWHERE CustomerId = @CustomerId;",
                Parameters = JsonConvert.SerializeObject(new List<SqlParam>
                {
                    new()
                    {
                        Name = "CustomerId",
                        Value = "[Customer].[CustomerId]"
                    }
                })
                
            });
            
            db.Insert(new TableDataQuery
            {
                Name = "supplier",
                Description = "Supplier by ID",
                DataSourceId = _dataSourcesIds[1],
                Statement = "SELECT SupplierName AS name, SupplierID\nFROM Suppliers\nWHERE SupplierID = @SupplierID;",
                Parameters = JsonConvert.SerializeObject(new List<SqlParam>
                {
                    new()
                    {
                        Name = "SupplierID",
                        Value = "2"
                    }
                })
                
            });
            
            db.Insert(new TableDataQuery
            {
                Name = "products",
                Description = "Products by ID",
                DataSourceId = _dataSourcesIds[1],
                Statement = "SELECT ProductName AS name, Unit AS unit, Price AS price \nFROM Products\nWHERE SupplierID = @SupplierID;",
                Parameters = JsonConvert.SerializeObject(new List<SqlParam>
                {
                    new()
                    {
                        Name = "SupplierID",
                        Value = "[supplier].[SupplierID]"
                    }
                })
                
            });
        }

        private void PopulateTableWorkflow()
        {
            using var db = new DocReuseDataConnection();

            var configCatWorkflow = new
            {
                Source = new {
                    Type = 1,
                    Document = new
                    {
                        Id = 1,
                        ProjectId = 1,
                        Name = "Document",
                    },
                    Composition = new
                    {
                        Id = 0
                    }
                },
                Parameters = new
                {
                    Type = 1
                },
                Generate = new
                {
                    Type = 1
                },
                SendTo = new
                {
                    Type = 1
                }
            };

            _dataSourcesIds.Add(db.InsertWithInt32Identity(new TableWorkflow
            {
                ProjectId = _projectIds[1],
                Name = "Cat catalog workflow",
                Configuration = JsonConvert.SerializeObject(configCatWorkflow) 
            }));
            
            
            var configCustomerWorkflow = new
            {
                Source = new {
                    Type = 1,
                    Document = new
                    {
                        Id = 3,
                        ProjectId = 2,
                        Name = "Document",
                    },
                    Composition = new
                    {
                        Id = 0
                    }
                },
                Parameters = new
                {
                    Type = 1
                },
                Generate = new
                {
                    Type = 1
                },
                SendTo = new
                {
                    Type = 1
                }
            };

            _dataSourcesIds.Add(db.InsertWithInt32Identity(new TableWorkflow
            {
                ProjectId = _projectIds[2],
                Name = "Customer letter workflow",
                Configuration = JsonConvert.SerializeObject(configCustomerWorkflow) 
            }));

            var configNorthwindWorkflow = new
            {
                Source = new {
                    Type = 1,
                    Document = new
                    {
                        Id = 5,
                        ProjectId = _projectIds[2],
                        Name = "Northwind products template"
                    },
                    Composition = new
                    {
                        Id = 0
                    }
                },
                Parameters = new
                {
                    Type = 1,
                    DataSource = new
                    {
                        Type = 1,
                    }
                },
                Generate = new
                {
                    Type = 1
                },
                SendTo = new
                {
                    Type = 1
                }
            };
            
            _dataSourcesIds.Add(db.InsertWithInt32Identity(new TableWorkflow
            {
                ProjectId = _projectIds[2],
                Name = "Northwind products workflow",
                Configuration = JsonConvert.SerializeObject(configNorthwindWorkflow) 
            }));
        }
    }
}