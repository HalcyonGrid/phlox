using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;

using MCompilerFrontend = InWorldz.Phlox.Glue.CompilerFrontend;
using InWorldz.Phlox.VM;
using System.IO;


namespace CompilerTests.FullTests
{
    [TestFixture]
    class Memory
    {
        [Test]
        public void TestLocalInitsMemoryAccountingBug()
        {
            string test
                = @"integer i;
                    float f;
                    vector v;
                    string s;
                    list l;
                    rotation r;

                    f() {
                      integer i;
                    }

                    g() {
                     integer i = 0;
                     for (i = 0; i < 999; i++) {
                        f();
                     }
                    }

                    integer h() {
                     g();
                     return 1;
                    }

                    integer k = h();

                    default { state_entry() {} }
                    ";

            MCompilerFrontend testFrontend = new MCompilerFrontend(new TestListener(), "../../../../grammar");
            CompiledScript script = testFrontend.Compile(test);

            Interpreter i = InterpRunner.Run(script);
            Assert.IsTrue(i.ScriptState.MemInfo.MemoryFree < MemoryInfo.MAX_MEMORY);
        }
    }
}
