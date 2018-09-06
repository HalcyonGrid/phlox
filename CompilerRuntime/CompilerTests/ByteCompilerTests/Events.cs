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
    class Events : BaseTest
    {
        [Test]
        public void TestGoodEventDef()
        {
            string test = @"
                            .statedef default
                            .evt default/touch_start: args=1, locals=0
                            halt
                            ";

            Compiler.Compile(new ANTLRStringStream(test));
            CompiledScript script = Compiler.Result;
            Assert.IsNotNull(script);

            Interpreter i = new Interpreter(script,null);
            i.TraceDestination = Listener.TraceDestination;
            while (i.ScriptState.RunState == RuntimeState.Status.Running)
            {
                i.Tick();
            }

            Assert.IsTrue(script.StateEvents[0][0].EventName == "touch_start"); 

            Assert.IsTrue(i.ScriptState.Operands.Count == 0); //stack should be empty
            Console.WriteLine(Listener.TraceDestination.ToString());
        }

        [Test]
        public void TestEventAlreadyDefined()
        {
            string test = @"
                            .statedef default
                            .evt default/touch_start: args=1, locals=0
                            halt
                            .evt default/touch_start: args=1, locals=0
                            halt
                            ";


            Compiler.Compile(new ANTLRStringStream(test));
            CompiledScript script = Compiler.Result;
            Assert.IsNull(script);

            Console.WriteLine(Listener.TraceDestination.ToString());
            Assert.IsTrue(Listener.MessagesContain("already defined"));
        }
    }
}
