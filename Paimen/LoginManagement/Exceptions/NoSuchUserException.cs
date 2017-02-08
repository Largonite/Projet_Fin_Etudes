using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginManagement.Exceptions
{
    public class NoSuchUserException : Exception
    {
        public NoSuchUserException() : base() { }
        public NoSuchUserException(string message) : base(message) { }
        public NoSuchUserException(string message, System.Exception inner) : base(message, inner) { }
    }
}