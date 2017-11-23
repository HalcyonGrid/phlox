using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;

using MCompilerFrontend = InWorldz.Phlox.Glue.CompilerFrontend;
using InWorldz.Phlox.VM;
using System.Text.RegularExpressions;

namespace CompilerTests.FullTests
{
    [TestFixture]
    class Generator
    {
        [Test]
        public void TestMultiParamEvent()
        {
            string test
                = @"default 
                    {
                        listen(integer chan, string name, key id, string text)
                        {
                            llSay(0, ""Script Running"");
                        }

                        timer()
                        {
                        }
                    }";

            TestListener listener = new TestListener();
            MCompilerFrontend testFrontend = new MCompilerFrontend(listener, "../../../../grammar");
            CompiledScript script = testFrontend.Compile(test);

            Assert.AreEqual(0, listener.ErrorCount);
        }

        [Test]
        public void TestPromotionForStorage()
        {
            string test
                = @"integer g() { return 0; }
                    f() { 
                        integer i;
                        float f = 1;
                        float h = g();
                        float j = i;
                        float l = i++;
                        float k = i - 5;

                        f = 1;
                        h = g();
                        j = i;
                        l = i++;
                        k = i - 5;
                    }

                    default { state_entry() {} }
                    ";

            TestListener listener = new TestListener();
            MCompilerFrontend testFrontend = new MCompilerFrontend(listener, "../../../../grammar", true);
            CompiledScript script = testFrontend.Compile(test);

            string byteCode = testFrontend.GeneratedByteCode;
            Console.WriteLine(byteCode);
            int castCount = new Regex("fcast").Matches(byteCode).Count;

            Assert.IsTrue(listener.HasErrors() == false);
            Assert.AreEqual(castCount, 10);
        }

        [Test]
        public void TestPromotionForFunctionCalls()
        {
            string test
                = @"g(float parm) { }
                    integer intfunc() { return 0; }

                    f() { 
                        integer i;
                        
                        g(1);
                        g(i);
                        g(i++);
                        g(i - 3);
                        g(intfunc());
                    }

                    default { state_entry() {} }
                    ";

            TestListener listener = new TestListener();
            MCompilerFrontend testFrontend = new MCompilerFrontend(listener, "../../../../grammar", true);
            CompiledScript script = testFrontend.Compile(test);

            string byteCode = testFrontend.GeneratedByteCode;
            Console.WriteLine(byteCode);
            int castCount = new Regex("fcast").Matches(byteCode).Count;

            Assert.IsTrue(listener.HasErrors() == false);
            Assert.AreEqual(castCount, 5);
        }

        [Test]
        public void TestPromotionForOps()
        {
            string test
                = @"g(float parm) { }
                    integer intfunc() { return 0; }

                    f() { 
                        integer i;
                        float f;
                        
                        f = i + 5.0;
                        f = 5.0 - i;
                        f = i == 1;
                        
                    }

                    default { state_entry() {} }
                    ";

            TestListener listener = new TestListener();
            MCompilerFrontend testFrontend = new MCompilerFrontend(listener, "../../../../grammar", true);
            CompiledScript script = testFrontend.Compile(test);

            string byteCode = testFrontend.GeneratedByteCode;
            Console.WriteLine(byteCode);
            int castCount = new Regex("fcast").Matches(byteCode).Count;

            Assert.IsTrue(listener.HasErrors() == false);
            Assert.AreEqual(castCount, 3);
        }

        [Test]
        public void TestImplicitCastForSubscriptAssignment()
        {
            string test
                = @"g(float parm) { }
                    integer intfunc() { return 0; }

                    f() { 
                        vector v;
                        v.x = intfunc();
                        float f = (v.y = 1);
                        
                    }

                    default { state_entry() {} }
                    ";

            TestListener listener = new TestListener();
            MCompilerFrontend testFrontend = new MCompilerFrontend(listener, "../../../../grammar", true);
            CompiledScript script = testFrontend.Compile(test);

            string byteCode = testFrontend.GeneratedByteCode;
            Console.WriteLine(byteCode);
            int castCount = new Regex("fcast").Matches(byteCode).Count;

            Assert.IsTrue(listener.HasErrors() == false);
            Assert.AreEqual(castCount, 2);
        }

        [Test]
        public void TestNoGenerateBooleanEval()
        {
            string test
                = @"integer intfunc() { return 0; }

                    f() { 
                        if (intfunc()) {
                        }

                        if (1) {
                        }

                        if (1 && 2 && 3) {
                        }
                        
                    }

                    default { state_entry() {} }
                    ";

            TestListener listener = new TestListener();
            MCompilerFrontend testFrontend = new MCompilerFrontend(listener, "../../../../grammar", true);
            CompiledScript script = testFrontend.Compile(test);

            string byteCode = testFrontend.GeneratedByteCode;
            Console.WriteLine(byteCode);
            int evalCount = new Regex("booleval").Matches(byteCode).Count;

            Assert.IsTrue(listener.HasErrors() == false);
            Assert.AreEqual(evalCount, 0);
        }

        [Test]
        public void TestDoGenerateBooleanEval()
        {
            string test
                = @"string strfunc() { return """"; }

                    f() { 
                        if (strfunc()) {
                        }

                        while (0.0) {
                        }

                        for (;"""";) {
                        }
                        
                        key k;
                        do {
                        } while (k);
                    }

                    default { state_entry() {} }
                    ";

            TestListener listener = new TestListener();
            MCompilerFrontend testFrontend = new MCompilerFrontend(listener, "../../../../grammar", true);
            CompiledScript script = testFrontend.Compile(test);

            string byteCode = testFrontend.GeneratedByteCode;
            Console.WriteLine(byteCode);
            int evalCount = new Regex("booleval").Matches(byteCode).Count;

            Assert.IsTrue(listener.HasErrors() == false);
            Assert.AreEqual(evalCount, 4);
        }

        [Test]
        public void TestGenerationOutputForTypePromotion()
        {
            string test
                = @"string strfunc() { return """"; }

                    f() { 
                        integer i;
                        float f;
                        
                        if (i < f) {}
                        if (i > f) {}
                        if (f > i) {}
                        if (f < i) {}
                        if (i >= f) {}
                        if (i <= f) {}
                        if (i < i) {}
                    }

                    default { state_entry() {} }
                    ";

            TestListener listener = new TestListener();
            MCompilerFrontend testFrontend = new MCompilerFrontend(listener, "../../../../grammar", true);
            CompiledScript script = testFrontend.Compile(test);

            string byteCode = testFrontend.GeneratedByteCode;
            Console.WriteLine(byteCode);

            Assert.IsTrue(listener.HasErrors() == false);
            Assert.AreEqual(new Regex("flt(?!e)").Matches(byteCode).Count, 2);
            Assert.AreEqual(new Regex("fgt(?!e)").Matches(byteCode).Count, 2);
            Assert.AreEqual(new Regex("fgte").Matches(byteCode).Count, 1);
            Assert.AreEqual(new Regex("flte").Matches(byteCode).Count, 1);
            Assert.AreEqual(new Regex("ilt").Matches(byteCode).Count, 1);
        }

        [Test]
        public void TestJumpDifferentSymbolSameName()
        {
            string test = @"
                        f() {
                            @a;
                            jump a;
                        }

                        g() {
                            @a;
                            jump a;
                        }
                    
                        default
                        {
                            touch_start(integer num) {
                            }
                        }";

            TestListener listener = new TestListener();
            MCompilerFrontend testFrontend = new MCompilerFrontend(listener, "../../../../grammar", true);
            CompiledScript script = testFrontend.Compile(test);

            string byteCode = testFrontend.GeneratedByteCode;
            Console.WriteLine(byteCode);

            Assert.IsTrue(listener.HasErrors() == false);
            Assert.AreEqual(new Regex("a_usrlbl__0").Matches(byteCode).Count, 2);
            Assert.AreEqual(new Regex("a_usrlbl__1").Matches(byteCode).Count, 2);
            Assert.AreEqual(new Regex("jmp a_usrlbl__0").Matches(byteCode).Count, 1);
            Assert.AreEqual(new Regex("jmp a_usrlbl__1").Matches(byteCode).Count, 1);
        }
    }
}
