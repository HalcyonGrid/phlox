﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Halcyon.Phlox.VM
{
    public class VMException : Exception
    {
        public VMException() : base()
        {

        }

        public VMException(string message)
            : base(message)
        {

        }
    }
}
