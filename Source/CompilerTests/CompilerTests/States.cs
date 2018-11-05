using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;

namespace CompilerTests.CompilerUnitTests
{
    [TestFixture]
    class States : BaseTest
    {
        [Test]
        public void TestGoodDefaultAndEvent()
        {
            string test =@"
                        default
                        {
                            touch_start(integer num) {
                                state default;
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestDuplicatedEvent()
        {
            string test = @"
                        default
                        {
                            touch_start(integer num) {
                            }
                            
                            touch_start(integer num) {
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Symbol 'touch_start()' already defined"));
        }

        [Test]
        public void TestWrongEventParams()
        {
            string test = @"
                        default
                        {
                            touch_start(float num) {
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Incorrect parameters"));
        }

        [Test]
        public void TestJumpInvalidState()
        {
            string test = @"
                        default
                        {
                            touch_start(integer num) {
                                state bam;
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Undefined state"));
        }

        [Test]
        public void TestDefaultMustBeFirstState()
        {
            string test = @"
                        state uhoh
                        {
                        }

                        default
                        {
                            touch_start(integer num) {
                                state default;
                            }
                        }";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 1);
            Assert.IsTrue(Listener.MessagesContain("Default state must be defined first."));
        }
    }
}
