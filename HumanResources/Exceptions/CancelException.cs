using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Exceptions
{
    /// <summary>
    /// Występuje w przypadku anulowania czynności
    /// </summary>
    class CancelException : Exception
    {
        public CancelException(string message) : base(message)
        {
        }
    }
}
