using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Antlr.Runtime;
using InWorldz.Phlox.VM;

namespace CompilerTests.ByteCompilerTests
{
    [TestFixture]
    class Jumps : BaseTest
    {
        [Test]
        public void TestCountingLoop()
        {
            string test = @"
                            .globals 1
                            .statedef default
                            iconst 0
                            gstore 0
                            loop:
                            gload 0
                            iconst 1
                            iadd
                            gstore 0
                            gload 0
                            trace
                            gload 0
                            iconst 10
                            ilt
                            brt loop
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
            Assert.IsTrue(Listener.MessagesContain($"1{Environment.NewLine}2{Environment.NewLine}3{Environment.NewLine}4{Environment.NewLine}5{Environment.NewLine}6{Environment.NewLine}7{Environment.NewLine}8{Environment.NewLine}9{Environment.NewLine}10{Environment.NewLine}"));
        }

        [Test]
        public void TestJmpAround()
        {
            string test = @"
                            .globals 1
                            .statedef default
                            jmp lbl2
                            lbl2:
                            iconst 0
                            jmp lbl3
                            lbl3:
                            iconst 1
                            jmp lbl4
                            lbl4:
                            iconst 2
                            sconst ""test""
                            trace
                            trace
                            trace
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
            Assert.IsTrue(Listener.MessagesContain($"test{Environment.NewLine}2{Environment.NewLine}1{Environment.NewLine}0"));
        }
    }
}
