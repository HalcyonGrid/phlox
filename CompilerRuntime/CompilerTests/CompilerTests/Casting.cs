using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;

namespace CompilerTests.CompilerUnitTests
{
    [TestFixture]
    class Casting : BaseTest
    {
        [Test]
        public void TestGoodCasts()
        {
            string test
                = @"
                    integer i = 4;
                    float f;
                    string s = ((string)4);
                    
                    integer f(integer i) {
                        return 0;
                    }

                    func() {
                        i = (integer)f;
                        f = i;
                        s = (string)f;
                        vector v = (vector)""<1,1,1,1>"";
                    }

                    default { state_entry() {} }
                         
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestBadCastVectorToInt()
        {
            string test
                = @"
                    f() {
                        integer i = (integer)<1,1,1>;
                    }

                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("Can not cast from vector to integer"));
        }

    }
}
