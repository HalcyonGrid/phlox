using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Antlr.Runtime;
using NUnit.Framework;

namespace CompilerTests.CompilerUnitTests
{
    class FunctionCalls : BaseTest
    {
        [Test]
        public void TestValidCalls()
        {
            string test
                = @"
                    integer f(integer i, string b, float c) { return 1; }
                    integer g(float f) { return 1; }
                    h(integer i) {}
                    
                    test() {
                        integer i = f(5, ""d"", 1);
                        g((float)""1.1"");

                        h(i);                        
                    }
                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestWrongNumberParams()
        {
            string test
                = @"
                    f(integer i, string s) {}
                    g() {
                        f(1);
                    }
                    default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("Function f() takes 2"));
        }

        [Test]
        public void TestWrongParamTypes()
        {
            string test
                = @"
                    f(integer i, string s) {}
                    g() {
                        f(""d"", 1);
                    }
                    default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("expected integer got string"));
        }

        [Test]
        public void TestWrongReturnType()
        {
            string test
                = @"
                    integer f(integer i, string s) {}
                    g() {
                        vector v;
                        v = f(1, ""1"");
                    }
                    default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("have incompatible types"));
        }
    }
}
