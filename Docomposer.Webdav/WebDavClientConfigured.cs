using System;
using System.Collections.Generic;
using System.Linq;
using Docomposer.Data.Util;
using WebDav;

namespace Docomposer.Webdav
{
    
    public class WebDavClientConfigured
    {
        private readonly IWebDavClient _webDavClient;

        public WebDavClientConfigured(string path)
        {
            var clientParams = new WebDavClientParams
            {
                BaseAddress = new Uri(path)
            };

            _webDavClient = new WebDavClient(clientParams);
        }

        public bool IsRessurceLocked(string path)
        {
            var result = _webDavClient.Propfind(path).Result;
            if (result.Resources.ToList().Count > 0)
            {
                var resource = result.Resources.ToList().First();

                if (resource.ActiveLocks.Count > 0)
                {
                    return true;
                }  
            }
            return false;
        }
        
        public List<string> GetUsersLockingResource(string path)
        {
            var users = new List<string>();
            
            var result = _webDavClient.Propfind(path).Result;
            if (result.Resources.ToList().Count > 0)
            {
                var resource = result.Resources.ToList().First();

                if (resource.ActiveLocks.Count > 0)
                {
                    foreach (var activeLock in resource.ActiveLocks)
                    {
                        users.Add(activeLock.Owner == null ? "System" : activeLock.Owner.Value);
                    }
                }  
            }

            return users;
        }

        public bool LockFile(string path)
        {
            var result = _webDavClient.Lock(path).Result;

            if (result.IsSuccessful)
            {
                return true;
            }
            return false;
        }

        public bool UnlockFile(string path)
        {
            var result = _webDavClient.Propfind(path).Result;
            if (result.Resources.ToList().Count > 0)
            {
                var resource = result.Resources.ToList().First();

                if (resource.ActiveLocks.Count > 0)
                {
                    foreach (var activeLock in resource.ActiveLocks)
                    {
                        var lockToken = activeLock.LockToken;
                        
                        var r = _webDavClient.Unlock(path, lockToken).Result;
                        
                        if (!r.IsSuccessful)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool Move(string sourcePath, string destPath)
        {
            var result = _webDavClient.Move(sourcePath, destPath).Result;

            return result.IsSuccessful;
        }

        public bool Delete(string uri)
        {
            var result = _webDavClient.Delete(uri).Result;

            return result.IsSuccessful;
        }
    }
}