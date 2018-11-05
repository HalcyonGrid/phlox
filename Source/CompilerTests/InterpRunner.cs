using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Halcyon.Phlox.VM;
namespace CompilerTests
{
    class InterpRunner
    {
        public static Interpreter Run(CompiledScript script)
        {
            if (script != null)
            {
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                Interpreter i = new Interpreter(script, null);
                i.TraceDestination = Console.Out;
                while (i.ScriptState.RunState == RuntimeState.Status.Running)
                {
                    i.Tick();
                }

                watch.Stop();
                System.Console.WriteLine("Execution: {0} seconds", watch.ElapsedMilliseconds / 1000.0);
                System.Console.WriteLine("Free Memory: {0} bytes", i.ScriptState.MemInfo.MemoryFree);

                return i;
            }

            return null;
        }
    }
}
