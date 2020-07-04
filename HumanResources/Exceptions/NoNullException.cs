using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Exceptions
{
    public class NoNullException : Exception
    {
        public NoNullException(string message) : base(message)
        {
        }
    }
}
