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
    class FuncCalls : BaseTest
    {
        [Test]
        public void TestCallFwdRef()
        {
            string test = @"
                            .globals 0
                            .statedef default

                            call func1()
                            halt
                            .def func1: args=0, locals=0
                            vconst <1.0,1.0,1.0>
                            trace
                            ret

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
        }

        [Test]
        public void TestCallWithParams()
        {
            string test = @"
                            .globals 0
                            .statedef default
                            iconst 20
                            call func1()
                            trace
                            sconst ""oh hai""
                            call func2()
                            halt
                            
                            .def func1: args=1, locals=1
                            load 0
                            trace
                            iconst 2
                            ret

                            .def func2: args=1, locals=1
                            load 0
                            store 1
                            sconst ""gnarly""
                            call func1()
                            pop
                            ret
                            
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
            Assert.IsTrue(Listener.MessagesContain($"20{Environment.NewLine}2{Environment.NewLine}gnarly{Environment.NewLine}"));
        }
    }
}
