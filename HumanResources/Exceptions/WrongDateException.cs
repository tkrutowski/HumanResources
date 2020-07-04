using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Exceptions
{
    class WrongDateTimeException : Exception
    {
        public WrongDateTimeException(string message) : base(message)
        {
        }
    }
}
