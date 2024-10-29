using System;

namespace Docomposer.Webdav
{
    public class WebDavException : Exception
    {
        public WebDavException(string message) : base("WebDav: " + message)
        {
            
        }
    }
}