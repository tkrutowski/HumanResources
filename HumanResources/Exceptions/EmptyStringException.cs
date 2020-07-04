using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Exceptions
{
    public class EmptyStringException : Exception
    {         
        public EmptyStringException(string message) : base(message)
        {
        }
    }
}
