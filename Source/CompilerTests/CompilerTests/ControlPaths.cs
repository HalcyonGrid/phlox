using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;

namespace CompilerTests.CompilerUnitTests
{
    class ControlPaths : BaseTest
    {
        [Test]
        public void TestGoodReturns()
        {
            string test
                = @"
                    integer f(integer i) {i++; return i;}
                    float g() {
                        if (1) {
                            return 1.1;
                        }
                        else {
                            return 2;
                        }
                    }

                    float h() {
                        if (FALSE) return 9;
                        if (TRUE) {
                            if (f(1) == 1) {
                                return 7;
                            }
                            
                            return 4;                            
                        } else {
                            if (f(2) == 5) {
                                return 8;
                            } else {
                                return 9;
                            }
                        }
                    }

                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestEmptyPath1()
        {
            string test
                = @"
                    integer f(integer i) {
                        if (i == 3) return 1;
                    }

                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Not all control paths return a value"));
        }

        [Test]
        public void TestEmptyPath2()
        {
            string test
                = @"
                    integer f(integer i) {
                        if (i == 3) return 1;
                        else jump bam;

                        return 1;
                        @bam;
                    }

                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Not all control paths return a value"));
        }

        [Test]
        public void TestEmptyPath3()
        {
            string test
                = @"
                    integer f(integer i) {
                        if (i == 3) {
                            if (i == 4) {
                                return 5;
                            }
                        }

                        return 1;
                        @bam;
                    }

                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Not all control paths return a value"));
        }

        [Test]
        public void TestEmptyPath4()
        {
            string test
                = @"
                    integer f(integer i) {
                        while (TRUE) {
                            return 1;
                        }
                    }

                    default{}
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Not all control paths return a value"));
        }

        [Test]
        public void TestEmptyPath5()
        {
            string test
                = @"
                    integer f(integer i) {
                        for (;;) {
                            return 1;
                        }
                    }

                    default{}
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Not all control paths return a value"));
        }

        [Test]
        public void TestEmptyPath6()
        {
            string test
                = @"
                    integer f(integer i) {
                        do {
                            return 1;
                        } while (TRUE);
                    }

                    default{}
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Not all control paths return a value"));
        }

        [Test]
        public void TestEmptyPath7()
        {
            string test
                = @"
                    integer f(integer i) {
                        if (TRUE) return 1;
                        else 
                        {
                            
                        }
                    }

                    default{}
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Not all control paths return a value"));
        }

        
    }
}
