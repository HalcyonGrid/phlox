using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;

namespace CompilerTests.CompilerUnitTests
{
    class Statements : BaseTest
    {
        [Test]
        public void TestBracketlessDoWhileStatement()
        {
            string test
                = @"f() {
                    integer x;
                    do x = 4; while (TRUE);
                    }

                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestWhileWithinBracketlessDoWhile()
        {
            string test
                = @"f() {
                    integer x;
                    do while(TRUE); while (TRUE);
                    }

                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestEmptyStatements()
        {
            string test
                = @"f() {
                        if (1);
                        while (1);
                        for (;;);
                        ;
                    }

                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

    }
}
