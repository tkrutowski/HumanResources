using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Exceptions
{
    class ErrorException : Exception
    {
        public ErrorException(string message) : base(message)
        {
        }
    }
}
