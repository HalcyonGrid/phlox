using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;

namespace CompilerTests.CompilerUnitTests
{
    class Jumps : BaseTest
    {
        [Test]
        public void TestGoodJump()
        {
            string test = @"
                        default
                        {
                            touch_start(integer num) {
                                @aLabel;
                                jump aLabel;
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestGoodJumpDifferentScope()
        {
            string test = @"
                        default
                        {
                            touch_start(integer num) {
                                @aLabel;

                                while (TRUE) {
                                    jump aLabel;
                                }
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestJumpToUnknownSymbol()
        {
            string test = @"
                        default
                        {
                            touch_start(integer num) {
                                jump aLabel;
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("Undefined label"));
        }

        [Test]
        public void TestJumpToOutsideSymbol()
        {
            string test = @"
                        f() {
                            @aLabel;
                        }
                    
                        default
                        {
                            touch_start(integer num) {
                                jump aLabel;
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("Undefined label"));
        }

        [Test]
        public void TestJumpSameNameDiffScope()
        {
            string test = @"
                        f() {

	                        {
		                        @a;
		                        jump a;
	                        }

	                        {
		                        @a;
		                        jump a;
	                        }
                        }

                        default { touch_start(integer num) {} }
                        ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }
    }
}
