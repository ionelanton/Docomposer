using System;
using System.IO;
using System.Threading;

namespace Docomposer.Utils.Converters
{
    public class LibreOfficePdfConverter : IPdfConverter
    {
        private static readonly string LibreOfficeExe = ThisApp.LibreOfficeWriter();
        private static readonly IFileHandler FileHandler = ThisApp.FileHandler();
       
        public bool ConvertDocxToPdf(string inputDocxFile, string outputPdfFile)
        {
            var tmpOutputDirectory = FileHandler.CombinePaths(ThisApp.TempDirectory(), Guid.NewGuid().ToString());

            if (!FileHandler.ExistsFile(inputDocxFile))
            {
                throw new ArgumentException($"File '{inputDocxFile}' not found");
            }
            
            if (!FileHandler.ExistsPath(tmpOutputDirectory))
            {
                FileHandler.CreatePath(tmpOutputDirectory);
            }
            
            var userTmpDirectory = FileHandler.CombinePaths(tmpOutputDirectory, Guid.NewGuid().ToString()).Replace("\\", "/");
            var slash = "";

            if (userTmpDirectory[0] != '/')
            {
                slash = "/";
            }

            var arguments =
                @$"--headless ""-env:UserInstallation=file://{slash}{userTmpDirectory}"" --convert-to ""pdf:writer_pdf_Export:{{\""ExportFormFields\"":{{\""type\"":\""boolean\"",\""value\"":\""false\""}}}}"" --outdir ""{tmpOutputDirectory}"" ""{inputDocxFile}""";
            
            var conversion = ProcessUtils.RunCommandWithTimeout(LibreOfficeExe, arguments, TimeSpan.FromMinutes(5));
            
            var success = false;
            
            while (!success)
            {
                success = conversion.IsCompletedSuccessfully;
                if (conversion.IsCanceled || conversion.IsFaulted)
                {
                    success = false;
                    break;
                }

                Thread.Sleep(200);
            }

            FileHandler.DeletePath(userTmpDirectory, true);

            if (success)
            {
                if (FileHandler.ExistsFile(outputPdfFile))
                {
                    FileHandler.Delete(outputPdfFile);
                }
                
                var generatedPdf = FileHandler.CombinePaths(tmpOutputDirectory, Path.GetFileNameWithoutExtension(inputDocxFile) + ".pdf");

                if (FileHandler.ExistsFile(generatedPdf))
                {
                    FileHandler.Move(generatedPdf, outputPdfFile);
                }

                if (!FileHandler.ExistsFile(outputPdfFile))
                {
                    success = false;
                }                
            }

            FileHandler.DeletePath(tmpOutputDirectory);

            return success;
        }
    }
}
