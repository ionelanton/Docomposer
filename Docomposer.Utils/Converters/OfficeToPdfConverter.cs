using System;
using System.Threading;

namespace Docomposer.Utils.Converters
{
    public class OfficeToPdfConverter : IPdfConverter
    {
        private static readonly IFileHandler FileHandler = ThisApp.FileHandler();
       
        public bool ConvertDocxToPdf(string inputDocxFile, string outputPdfFile)
        {
            if (!FileHandler.ExistsFile(inputDocxFile))
            {
                throw new ArgumentException($"File '{inputDocxFile}' not found");
            }
            
            if (FileHandler.ExistsFile(outputPdfFile))
            {
                FileHandler.Delete(outputPdfFile);
            }

            Console.WriteLine("Started");
            var officeToPdfExe = FileHandler.CombinePaths(ThisApp.BaseDirectory(), "OfficeToPdf", "OfficeToPDF.exe");

            var conversion = ProcessUtils.RunCommandWithTimeout(officeToPdfExe,
                $"/readonly /noquit \"{inputDocxFile}\" \"{outputPdfFile}\"", TimeSpan.FromMinutes(5));
            
            var success = false;
            
            while (!success)
            {
                success = conversion.IsCompletedSuccessfully;
                if (conversion.IsCanceled || conversion.IsFaulted)
                {
                    success = false;
                    break;
                }

                Thread.Sleep(100);
            }

            Console.WriteLine("Stopped");
            
            if (success && !FileHandler.ExistsFile(outputPdfFile))
            {
                success = false;
            }

            return success;
        }
    }
}
