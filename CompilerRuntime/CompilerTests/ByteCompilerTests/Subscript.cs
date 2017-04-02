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
    class Subscript : BaseTest
    {
        [Test]
        public void TestSubscriptAccess()
        {
            string test = @"
                            .globals 2
                            .statedef default
                            vconst <1.1,1.2,1.3>
                            gstore 0
                            gload.sub 0,0
                            trace
                            gload.sub 0,1
                            trace
                            gload.sub 0,2
                            trace
                            rconst <1.1,1.2,1.3,1.4>
                            gstore 1
                            gload.sub 1,0
                            trace
                            gload.sub 1,1
                            trace
                            gload.sub 1,2
                            trace
                            gload.sub 1,3
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
            Assert.IsTrue(Listener.MessagesContain($"1.1{Environment.NewLine}1.2{Environment.NewLine}1.3{Environment.NewLine}1.1{Environment.NewLine}1.2{Environment.NewLine}1.3{Environment.NewLine}1.4"));
        }

        [Test]
        public void TestSubscriptIncrement()
        {
            string test = @"
                            .globals 1
                            .statedef default
                            vconst <1.1,1.2,1.3>
                            gstore 0
                            fpreinc.g.sub 0,1
                            trace
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
            Assert.IsTrue(Listener.MessagesContain($"2.2{Environment.NewLine}<1.1, 2.2, 1.3>"));
        }
    }
}
