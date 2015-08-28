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
    class BytecodeAssembler
    {
        [Test]
        public void TestGeneratesFinalRetForEvent()
        {
            string test
                = @"default 
                    {
                        state_entry()
                        {
                            llSay(0, ""Script Running"");
                        }
                    }";

            MCompilerFrontend testFrontend = new MCompilerFrontend(new TestListener(), "..\\..\\..\\..\\grammar");
            CompiledScript script = testFrontend.Compile(test);

            Assert.Contains(93, script.ByteCode);
        }

        [Test]
        public void TestLauksLarrowScript()
        {
            string test;
            using (StreamReader rdr = File.OpenText("..\\..\\..\\..\\grammar\\test_files\\Lauks_Larrow_Main.lsl"))
            {
                test = rdr.ReadToEnd();
            }

            TestListener listener = new TestListener();
            MCompilerFrontend testFrontend = new MCompilerFrontend(listener, "..\\..\\..\\..\\grammar");
            CompiledScript script = testFrontend.Compile(test);

            Assert.IsTrue(listener.HasErrors() == false);
        }
    }
}
