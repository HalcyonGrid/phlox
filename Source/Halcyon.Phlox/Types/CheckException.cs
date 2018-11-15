using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Halcyon.Phlox.Types
{
    public class CheckException : Exception
    {
        public CheckException()
            : base()
        {

        }

        public CheckException(string message)
            : base(message)
        {

        }
    }
}
