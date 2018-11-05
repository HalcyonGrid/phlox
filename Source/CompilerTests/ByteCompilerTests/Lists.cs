using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;
using Halcyon.Phlox.VM;

namespace CompilerTests.ByteCompilerTests
{
    [TestFixture]
    class Lists : BaseTest
    {
        [Test]
        public void TestBuildList()
        {
            string test = @"
                            .globals 1
                            .statedef default
                            fconst 1.1
                            fconst 3.2
                            sconst ""hi""
                            sconst ""\""""
                            iconst 5
                            buildlist 5
                            trace
                            halt

                            .evt default/state_entry: args=0, locals=0
                            ret
                            ";

            Compiler.Compile(new ANTLRStringStream(test));
            CompiledScript script = Compiler.Result;
            Assert.IsNotNull(script);

            Interpreter i = new Interpreter(script, null);
            i.TraceDestination = Listener.TraceDestination;
            while (i.ScriptState.RunState == RuntimeState.Status.Running)
            {
                i.Tick();
            }

            Assert.IsTrue(i.ScriptState.Operands.Count == 0); //stack should be empty
            Console.WriteLine(Listener.TraceDestination.ToString());
            Assert.IsTrue(Listener.MessagesContain("[1.1,3.2,hi,\",5]"));
        }

        [Test]
        public void TestConcatList()
        {
            string test = @"
                            .globals 1
                            .statedef default
                            fconst 1.1
                            buildlist 1
                            gstore 0
                            gload 0
                            iconst 25
                            list.append
                            gstore 0
                            sconst ""omg""
                            gload 0
                            list.prepend
                            gstore 0
                            gload 0
                            trace
                            halt

                            .evt default/state_entry: args=0, locals=0
                            ret
                            ";

            Compiler.Compile(new ANTLRStringStream(test));
            CompiledScript script = Compiler.Result;
            Assert.IsNotNull(script);

            Interpreter i = new Interpreter(script, null);
            i.TraceDestination = Listener.TraceDestination;
            while (i.ScriptState.RunState == RuntimeState.Status.Running)
            {
                i.Tick();
            }

            Assert.IsTrue(i.ScriptState.Operands.Count == 0); //stack should be empty
            Console.WriteLine(Listener.TraceDestination.ToString());
            Assert.IsTrue(Listener.MessagesContain("[omg,1.1,25]"));
        }
    }
}
