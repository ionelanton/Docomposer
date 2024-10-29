using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Docomposer.Utils
{
    public interface IFileHandler
    {
        public string CombinePaths(params string[] parts);
        public void CreatePath(string path);
        public bool ExistsPath(string path);
        public void DeletePath(string path, bool recursive = false);
        public bool ExistsFile(string filePath);
        public FileInUseDetails IsFileInUse(string filePath);
        public void Move(string filePath, string newFilePath);
        public void Copy(string filePath, string newFilePath);
        public void Delete(string filePath);
        public DateTime GetLastWriteTimeUtc(string filePath);
        public void SetLastWriteTime(string filePath, DateTime dateTime);
        public bool IsWordDocumentOpen(string documentPath, string documentName);

        public byte[] ReadAllBytes(string filePath);
    }

    public class FileSystemHandler : IFileHandler
    {
        public string CombinePaths(params string[] parts)
        {
            var path = "";
            foreach (var part in parts)
            {
                path = Path.Combine(path, part);
            }

            return path;
        }

        public void CreatePath(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool ExistsPath(string path)
        {
            return Directory.Exists(path);
        }

        public void DeletePath(string path, bool recursive = false)
        {
            Directory.Delete(path, recursive);
        }

        public bool ExistsFile(string filePath)
        {
            return File.Exists(filePath);
        }

        public FileInUseDetails IsFileInUse(string filePath)
        {
            var file = new FileInfo(filePath);
            
            try
            {
                using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return new FileInUseDetails
                {
                    InUse = true,
                    Users = Environment.UserName
                };
            }

            //file is not locked
            return new FileInUseDetails
            {
                InUse = false,
                Users = ""
            };
        }

        public void Move(string filePath, string newFilePath)
        {
            File.Move(filePath, newFilePath);
        }

        public void Copy(string filePath, string newFilePath)
        {
            File.Copy(filePath, newFilePath);
        }

        public void Delete(string filePath)
        {
            File.Delete(filePath);
        }

        public DateTime GetLastWriteTimeUtc(string filePath)
        {
            return File.GetLastWriteTimeUtc(filePath);
        }

        public void SetLastWriteTime(string filePath, DateTime dateTime)
        {
            File.SetLastWriteTime(filePath, dateTime);
        }

        public bool IsWordDocumentOpen(string documentPath, string documentName)
        {
            var wordInstances = Process.GetProcessesByName("WINWORD");

            foreach (var instance in wordInstances)
            {
                if (instance.MainWindowTitle.Contains(documentName))
                {
                    var searchPaths = new List<string> { CombinePaths(documentPath, $"~${documentName}.docx") };

                    if (documentName.Length > 2)
                    {
                        searchPaths.Add(CombinePaths(documentPath, $"~${documentName[1..]}.docx"));
                        searchPaths.Add(CombinePaths(documentPath, $"~${documentName[2..]}.docx"));
                    }

                    if (searchPaths.Any(ExistsFile))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public byte[] ReadAllBytes(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }
    }

    public class WebDavFileHandler : IFileHandler
    {
        public string CombinePaths(params string[] parts)
        {
            throw new NotImplementedException();
        }

        public void CreatePath(string path)
        {
            throw new NotImplementedException();
        }

        public bool ExistsPath(string path)
        {
            throw new NotImplementedException();
        }

        public void DeletePath(string path, bool recursive = false)
        {
            throw new NotImplementedException();
        }

        public bool ExistsFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public FileInUseDetails IsFileInUse(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Move(string filePath, string newFilePath)
        {
            throw new NotImplementedException();
        }

        public void Copy(string filePath, string newFilePath)
        {
            throw new NotImplementedException();
        }

        public void Delete(string filePath)
        {
            throw new NotImplementedException();
        }

        public DateTime GetLastWriteTimeUtc(string filePath)
        {
            throw new NotImplementedException();
        }

        public void SetLastWriteTime(string filePath, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public bool IsWordDocumentOpen(string documentPath, string documentName)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadAllBytes(string filePath)
        {
            throw new NotImplementedException();
        }
    }

    public class FileInUseDetails
    {
        public bool InUse { get; set; }
        public string Users { get; set; }
    }

    public static class FileHandlerExtensions
    {
        public static bool IsWebDavPath(this string path)
        {
            return path.ToLower().StartsWith("http");
        }
    }
    
}