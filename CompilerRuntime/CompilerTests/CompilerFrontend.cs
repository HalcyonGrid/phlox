using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InWorldz.Phlox.Compiler;
using InWorldz.Phlox.Types;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using InWorldz.Phlox.Compiler.BranchAnalyze;

namespace CompilerTests
{
    class CompilerFrontend
    {
        private System.IO.TextWriter _traceDestination;
        public System.IO.TextWriter TraceDestination
        {
            get
            {
                return _traceDestination;
            }

            set
            {
                _traceDestination = value;
            }
        }

        private ILSLListener _listener;
        public ILSLListener Listener
        {
            get
            {
                return _listener;
            }

            set
            {
                _listener = value;
            }
        }

        public InWorldz.Phlox.Glue.CompilerFrontend Compile(ICharStream input)
        {
            InWorldz.Phlox.Glue.CompilerFrontend frontEnd = new InWorldz.Phlox.Glue.CompilerFrontend(_listener, "..\\..\\..\\..\\grammar", true);
            frontEnd.OutputASTGraph = true;

            frontEnd.Compile(input);

            return frontEnd;
        }
    }
}
