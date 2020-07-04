using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Exceptions
{
    public class WrongSizeStringException : Exception
    {
        public WrongSizeStringException(string message) : base(message)
        {
        }
    }
}
