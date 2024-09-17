using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException()
        {

        }
        public CustomException(string exceptionMessage) : base(exceptionMessage)
        {

        }
    }
}
