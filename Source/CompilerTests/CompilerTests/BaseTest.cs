using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace CompilerTests.CompilerUnitTests
{
    class BaseTest
    {
        private CompilerFrontend _compiler;
        private TestListener _listener;

        protected CompilerFrontend Compiler
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
            _compiler = new CompilerFrontend(System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "grammar"));
            _listener = new TestListener();
            _compiler.Listener = _listener;
            _compiler.TraceDestination = Console.Error;
        }
    }
}
