using System;
using System.Collections.Generic;
using Docomposer.Data.Databases.DataStore.Tables;
using Docomposer.Utils;

namespace Docomposer.Data.Util
{
    public class FilePopulator : IFilePopulator
    {
        private string BasePath { get; }
        private readonly IFileHandler _fileHandler = ThisApp.FileHandler();
        private const string SolutionDir = "Docomposer";

        public FilePopulator(string path = "")
        {
            if (path.Length == 0)
            {
                BasePath = ThisApp.DocReuseDocumentsPath();
            }
            else
            {
                BasePath = path;
            }
        }

        public void Populate()
        {
            if (_fileHandler.ExistsPath(BasePath))
            {
                _fileHandler.DeletePath(BasePath, true);
            }

            _fileHandler.CreatePath(BasePath);

            var projectIds = new List<int> { 2, 3 };

            foreach (var id in projectIds)
            {
                _fileHandler.CreatePath(BasePath + $"/{id}/{DirName.Sections}");
                _fileHandler.CreatePath(BasePath + $"/{id}/{DirName.Templates}");
            }

            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var index = appDir.IndexOf(SolutionDir, StringComparison.InvariantCulture);

            _fileHandler.CombinePaths(ThisApp.AppDataDirectory(), "Resources");

            var sourceDir = "";
            try
            {
                sourceDir = appDir.Remove(index) + _fileHandler.CombinePaths(SolutionDir, "Docomposer.Utils", "Resources");
            }
            catch (Exception)
            {
                sourceDir = _fileHandler.CombinePaths(ThisApp.AppDataDirectory(), "Resources");
            }

            // Sections
            var catCatalogSections = new List<string>
            {
                "Cat 1.docx", "Cat 2.docx", "Cat 3.docx"
            };
            _populateSections(sourceDir, 2, catCatalogSections);

            var customerLetterSections = new List<string>
            {
                "Customer letter header.docx", "Customer letter content.docx"
            };
            _populateSections(sourceDir, 3, customerLetterSections);

            var northwindProductsSection = new List<string>
            {
                "Northwind products section.docx"
            };
            _populateSections(sourceDir, 3, northwindProductsSection);
            
            var invoiceSections = new List<string>
            {
                "Customer invoice list.docx"
            };
            _populateSections(sourceDir, 3, invoiceSections);
            
            // Documents
            var catCatalogDocuments = new List<string>
            {
                "01. Cat catalog cover.docx", 
                "02. Cat catalog content.docx"
            };
            _populateDocuments(sourceDir, 2, catCatalogDocuments);

            var customerLetterDocuments = new List<string> { "Customer sale promotion.docx" };
            _populateDocuments(sourceDir, 3, customerLetterDocuments);

            var customerInvoiceDocuments = new List<string> { "Customer invoices.docx" };
            _populateDocuments(sourceDir, 3, customerInvoiceDocuments);

            var northwindProductDocuments = new List<string> { "Northwind products template.docx" };
            _populateDocuments(sourceDir, 3, northwindProductDocuments);

            // Data sources
            _populateDataSources(sourceDir, 3, DataSourceEnum.Excel, "Chinook.xlsx");
            _populateDataSources(sourceDir, 3, DataSourceEnum.Excel, "Northwind.xlsx");
        }

        private void _populateSections(string resourcesDir, int projectId, List<string> sections)
        {
            var sectionsPath = _fileHandler.CombinePaths(resourcesDir, DirName.Documents, projectId.ToString(), DirName.Sections);

            foreach (var section in sections)
            {
                _fileHandler.Copy(sectionsPath + @"/" + section,
                    _fileHandler.CombinePaths(BasePath + $"/{projectId}/{DirName.Sections}", section));
            }
        }

        private void _populateDocuments(string resourcesDir, int projectId, List<string> documents)
        {
            var documentsPath = _fileHandler.CombinePaths(resourcesDir, DirName.Documents, projectId.ToString(), DirName.Templates);
            
            foreach (var document in documents)
            {
                _fileHandler.Copy(documentsPath + @"/" + document,
                    _fileHandler.CombinePaths(BasePath + $"/{projectId}/{DirName.Templates}", document));
            }
        }

        private void _populateDataSources(string resourcesDir, int projectId, DataSourceEnum type, string fileName)
        {
            var dataSourcesPath = _fileHandler.CombinePaths(resourcesDir, DirName.DataSources, projectId.ToString());
            
            _fileHandler.CreatePath(_fileHandler.CombinePaths(BasePath, $"{DirName.DataSources}", $"{projectId}", $"{(int)type}"));
            _fileHandler.Copy( _fileHandler.CombinePaths(dataSourcesPath, fileName),
            _fileHandler.CombinePaths(BasePath, $"{DirName.DataSources}", $"{projectId}", $"{(int)type}", fileName));
            
        }
    }
}