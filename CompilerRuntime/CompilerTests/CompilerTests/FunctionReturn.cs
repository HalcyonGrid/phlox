using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;


namespace CompilerTests.CompilerUnitTests
{
    [TestFixture]
    class FunctionReturn : BaseTest
    {
        [Test]
        public void TestCorrectReturnType()
        {
            string test = @"
                            integer f() {
                                return 1;
                            }

                            integer g() {
                                integer i;
                                return i;
                            }
                            
                            h() {
                                return;
                            }
    
                            float i() {
                                return (float)6;
                            }
                            
                            integer j() {
                                return f();
                            }

                            default { state_entry() {} }
                            ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestBadReturnType()
        {
            string test = @"
                            integer f() {
                                return 3.2;
                            }
                            default { state_entry() {} }
                            ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Invalid return type"));
        }

        [Test]
        public void TestVoidReturnWithTypeExpected()
        {
            string test = @"
                            integer f() {
                                return;
                            }
                            default { state_entry() {} }
                            ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("must return a value"));
        }

        [Test]
        public void TestReturnValueInVoidFunction()
        {
            string test = @"
                            f() {
                                return 1;
                            }
                            default { state_entry() {} }
                            ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Invalid return type"));
        }


    }
}
