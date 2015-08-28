using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;

namespace CompilerTests.CompilerUnitTests
{
    [TestFixture]
    class Assignment : BaseTest
    {
        [Test]
        public void TestGoodAssignments()
        {
            string test
                = @"
                    integer i = 4;
                    float f;
                    string s = ((string)4);
                    
                    func() {
                        f += 0.3;
                        i *= 4;
                        s += ""\nd"";

                        vector v;
                        v *= <1,1,1,1>;

                        integer j = -1;
                        j = -2;
                    }
                    default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestBadTypeForDiv()
        {
            string test
                = @"
                    string s = ((string)4);
                    
                    func() {
                        integer i;
                        i /= <1,1,1>;
                        
                    }
                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("not a valid operation between integer and vector"));
        }

        [Test]
        public void TestBadTypeForMul()
        {
            string test
                = @"
                    string s = ((string)4);
                    
                    func() {
                        vector v;
                        rotation r;
                        
                        r *= v;
                        
                    }
                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("not a valid operation between rotation and vector"));
        }

        [Test]
        public void TestBadTypeForMod()
        {
            string test
                = @"
                    func() {
                        vector v;
                        
                        v %= 5;
                        
                    }
                    default {}
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("not a valid operation between vector and integer"));
        }

        [Test]
        public void TestAssignBeforeDef()
        {
            string test
                = @"
                    func() {
                        v = <1,1,1>;
                        vector v;
                        
                    }
                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount > 0);
            Assert.IsTrue(Listener.MessagesContain("can not be used before it is defined"));
        }

        [Test]
        public void TestGoodAssignmentBeforeAndAfterShadowing()
        {
            string test
                = @"
                    func(vector v) {
                        v = <1,1,1>;
                        vector v;
                        v = <2,2,2>;
                    }
                    default { state_entry() {} }
                    ";

            InWorldz.Phlox.Glue.CompilerFrontend fe = Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
            Assert.IsTrue(fe.GeneratedByteCode.Contains("store 0"));
            Assert.IsTrue(fe.GeneratedByteCode.Contains("store 1"));
        }

        [Test]
        public void TestGoodAssignmentReferenceToParamFromShadow()
        {
            string test
                = @"
                    func(vector v) {
                        v = <1.0, 1.0, 1.0>;
                        vector v = v;
                        v = <1.0, 1.0, 1.0>;
                    }
                    default { state_entry() {} }
                    ";

            InWorldz.Phlox.Glue.CompilerFrontend fe = Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
            Console.WriteLine(fe.GeneratedByteCode);
            Assert.IsTrue(fe.GeneratedByteCode.Contains("load 0"));
            Assert.IsTrue(fe.GeneratedByteCode.Contains("store 1"));
        }

        [Test]
        public void TestGoodAssignBeforeDefWithGlobal()
        {
            string test
                = @"
                    vector v;

                    func() {
                        v = <1,1,1>;
                        vector v;
                        
                    }
                    default { state_entry() {} }
                    ";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }

        [Test]
        public void TestAssignmentWithParensSurrounding()
        {
            string test
                = @"
                    func() {
                        integer f;
                        (f = 4);
                    }
                    default { state_entry() {} }
";

            Compiler.Compile(new ANTLRStringStream(test));
            Assert.IsTrue(Listener.ErrorCount == 0);
        }
    }
}
