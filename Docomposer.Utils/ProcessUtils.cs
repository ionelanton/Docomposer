using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using RunProcessAsTask;

namespace Docomposer.Utils
{
    public static class ProcessUtils
    {
        public static async Task<ProcessResults> RunCommandWithTimeout(string filename, string arguments, TimeSpan timeout)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"{filename}");
            }
            
            var processStartInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
            };

            //todo: see also https://github.com/Tyrrrz/CliWrap
            using (var cancellationTokenSource = new CancellationTokenSource(timeout))
            {
                return await ProcessEx.RunAsync(processStartInfo, cancellationTokenSource.Token);
            }
        }

        public static string EmptyDocxFile()
        {
            return ThisApp.FileHandler().CombinePaths(ThisApp.BaseDirectory(), "Util", "Resources", "empty.docx");
        }

        public static void OpenExistingDocxInMicrosoftWord(string docxFile)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = WordExeFilePath(),
                Arguments = $@"/t ""{docxFile}"""
            };

            ProcessEx.RunAsync(processStartInfo);
        }

        public static string WordExeFilePath()
        {
            var wordExeFilePath = $@"{_getMicrosoftOfficePath()}\WINWORD.EXE";

            if (File.Exists(wordExeFilePath))
            {
                return wordExeFilePath;
            }

            throw new ApplicationException("Microsoft Word not found!");
        }
        
        public static string ExcelExeFilePath()
        {
            var excelExeFilePath = $@"{_getMicrosoftOfficePath()}\EXCEL.EXE";
            
            if (File.Exists(excelExeFilePath))
            {
                return excelExeFilePath;
            }

            throw new ApplicationException("Microsoft Excel not found!");
        }

        private static string _getMicrosoftOfficePath()
        {
            var msOfficePath = "";
            
            var versionKey = Registry.ClassesRoot.OpenSubKey(@"Word.Application\CurVer");

            var version = versionKey?.GetValue("")?.ToString();

            if (version == null) return msOfficePath;
            
            var msOfficeVersionNumber = version.Split(".").ToList().Last();
                
            var paths = new List<string>
            {
                $@"C:\Program Files\Microsoft Office\root\Office{msOfficeVersionNumber}",
                $@"C:\Program Files (x86)\Microsoft Office\root\Office{msOfficeVersionNumber}"
            };

            foreach (var path in paths.Where(Directory.Exists))
            {
                msOfficePath = path;
                break;
            }

            return msOfficePath;
        }
    }
}
