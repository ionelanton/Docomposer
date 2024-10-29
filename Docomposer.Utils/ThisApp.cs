using System;
using System.IO;
using Docomposer.Utils.Converters;

namespace Docomposer.Utils
{
    public static class ThisApp
    {
        private static string _dataStoreName = "app.data";
        private static string _keyStoreName = "app.key";

        public static string AppEnvironment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        }

        public static string Distribution()
        {
            return DefaultDistribution.Desktop;
        }

        public static IFileHandler FileHandler()
        {
            return new FileSystemHandler();
        }

        public static IPdfConverter PdfConverter()
        {
            //return new OfficeToPdfConverter();
            return new LibreOfficePdfConverter();
        }

        public static string BaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
        public static string AppDirectory()
        {
            // windows
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                DirName.Docomposer);
            //todo: linux
        }

        public static string AppDataDirectory()
        {
            // windows
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                DirName.Docomposer);
        }
        
        public static string DocReuseDocumentsPath()
        {
            return Path.Combine(AppDataDirectory(), DirName.Documents);
        }
        
        public static string DocReuseCacheDirectory()
        {
            return Path.Combine(AppDataDirectory(), DirName.Cache);
        }

        public static string TempDirectory()
        {
            return Path.Combine(Path.GetTempPath(), DirName.Docomposer);
        }

        public static string LibreOfficeWriter()
        {
            return @"C:\Program Files\LibreOffice\program\soffice.com";
        }
        
        public static string MicrosoftOfficeWord()
        {
            return ProcessUtils.WordExeFilePath();
        }
        
        public static string MicrosoftOfficeExcel()
        {
            return ProcessUtils.ExcelExeFilePath();
        }

        public static string WebDavBaseUrl()
        {
            return ThisAppConfiguration.Configuration["WebDAV:BaseUrl"];
        }

        public static string SqliteMemoryConnectionString()
        {
            return "Data Source=:memory:;";
        }
        
        // public static string WebDavPhysicalPath()
        // {
        //     return ThisAppConfiguration.Configuration["WebDAV:PhysicalPath"];
        // }

        public static string DataStore(string name = "")
        {
            if (name != string.Empty)
            {
                _dataStoreName = name;
            }

            return _dataStoreName;
        }

        public static class AppSecurity
        {
            public static string DataStorePassword { get; set; }
        }

        public static string KeyStore(string name = "")
        {
            if (name != string.Empty)
            {
                _keyStoreName = name;
            }

            return _keyStoreName;
        }

        public static string KeyStoreConnectionString()
        {
            return $"Data Source={Path.Combine(AppDataDirectory(), KeyStore())};Mode=ReadWriteCreate;Pooling=True;";
        }
    }
}