using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginManagement.Exceptions
{
    public class DBException : Exception
    {
        public DBException() : base() { }
        public DBException(string message) : base(message) { }
        public DBException(string message, System.Exception inner) : base(message, inner) { }
    }
}