using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Antlr.Runtime;

namespace CompilerTests.CompilerUnitTests
{
    [TestFixture]
	class UnaryOps : BaseTest
	{
        [Test]
        public void TestGoodUnaryAssignments()
        {
            string test
                = @"
                    integer f(integer i) {i++; return i;}
                    integer i;
                    integer j = !i;
                    integer k = 0;
                    integer l = ~k;
                    integer m = !f(1);
                    integer n = ++k;
                    
                    g() {
                        k = !j;
                        integer l = k++;
                        i++;
                    }
                            default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestPrecrementNotAssignable()
        {
            string test = @"
                            f() {
                                string j;
                                integer i = ++((integer)j);
                            }
                            default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("not assignable"));
        }

        [Test]
        public void TestPostIncrementNotAssignable()
        {
            string test = @"
                            f() {
                                string j;
                                integer i = ((integer)j)++;
                            }
                            default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("not assignable"));
        }

        [Test]
        public void TestIncrementBadType()
        {
            string test = @"
                            f() {
                                string j;
                                j++;
                                ++j;
                            }
                            default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 2);
            Assert.IsTrue(Listener.MessagesContain("not valid for type"));
        }

        [Test]
        public void TestGoodSubscripts()
        {
            string test = @"
                            f() {
                                vector v;
                                rotation r;
                                
                                float f = v.x++;
                                f = v.y;
                                f = v.z++;
                                
                                float f2 = r.s;
                                f2++;
                                f2 = r.x++;
                                r.y++;
                                r.z++;
                            }
                            default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestBadSubscriptName()
        {
            string test = @"
                            f() {
                                vector v;
                                float f = v.g;
                            }
                            default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("Invalid subscript"));
        }

        [Test]
        public void TestSubscriptOnInvalidType()
        {
            string test = @"
                            f() {
                                float f;
                                f.x++;
                            }
                            default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("requires a vector or rotation variable"));
        }

        [Test]
        public void TestSubscriptOnNonVariable()
        {
            string test = @"
                            f() {
                                string s;
                                float f = ((vector)s).x;
                            }
                            default { state_entry() {} }
";


            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("requires a vector or rotation variable"));
        }
	}
}
