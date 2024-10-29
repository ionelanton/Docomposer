using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Docomposer.Core.Util
{
    public static class FileUtil
    {
        public static bool IsFileLocked(string filePath, int timeoutInSeconds = 10)
        {
            var time = Stopwatch.StartNew();
            
            while (time.ElapsedMilliseconds < timeoutInSeconds * 1000)
            {
                try
                {
                    var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    if (stream.Length > 0)
                    {
                        return true;
                    }
                }
                catch (IOException e)
                {
                    Thread.Sleep(100);
                    // access error
                    if (e.HResult != -2147024864)
                        throw;
                }
            }
            return false;
        }
    }
}
