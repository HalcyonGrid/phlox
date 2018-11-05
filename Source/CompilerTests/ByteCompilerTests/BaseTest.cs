using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace CompilerTests.ByteCompilerTests
{
    class BaseTest
    {
        private ByteCompilerFrontend _compiler;
        private TestListener _listener;

        protected ByteCompilerFrontend Compiler
        {
            get
            {
                return _compiler;
            }
        }

        protected TestListener Listener
        {
            get
            {
                return _listener;
            }
        }

        [SetUp]
        public void Setup()
        {
            _compiler = new ByteCompilerFrontend();
            _listener = new TestListener();
            _compiler.Listener = _listener;
            _compiler.TraceDestination = Console.Out;
        }
    }
}
