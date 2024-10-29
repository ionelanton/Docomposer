using System;

namespace Docomposer.Core.Util
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base("Api: " + message)
        {
            
        }
    }
}