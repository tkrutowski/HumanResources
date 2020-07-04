using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Exceptions
{
    class NoAccessException : Exception
    {
        public NoAccessException(string message) : base(message)
        {
        }
    }
}
