using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Antlr.Runtime;
using Antlr.Runtime.Tree;

using Halcyon.Phlox.Compiler;
using Halcyon.Phlox.VM;
using Halcyon.Phlox.Types;
using Halcyon.Phlox.Serialization;
using System.IO;

namespace CompilerRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            ICharStream input = null;

            if (args.Length > 0)
            {
                if (args[0] == "bytecompiler")
                {
                    input = new ANTLRInputStream(Console.OpenStandardInput());
                    ByteCompilerFrontend fe = new ByteCompilerFrontend();
                    fe.Listener = new DefaultLSLListener();
                    fe.Compile(input);

                    CompiledScript script = fe.Result;
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
                        System.Console.WriteLine(watch.ElapsedMilliseconds/1000.0);
                    }
                }
                else
                {
                    foreach (string arg in args)
                    {
                        Console.WriteLine("Compiling: " + arg);

                        input = new ANTLRFileStream(arg);
                        CompilerFrontend fe = new CompilerFrontend();
                        fe.TraceDestination = Console.Out;
                        fe.Compile(input);
                    }
                }
            }
            else
            {
                ILSLListener listener = new DefaultLSLListener();
                LSLListenerTraceRedirectorMono redirector = new LSLListenerTraceRedirectorMono(listener);

                input = new ANTLRInputStream(Console.OpenStandardInput());
                CompilerFrontend fe = new CompilerFrontend();
                fe.TraceDestination = redirector;
                fe.Listener = listener;

                Console.WriteLine("** compilation output **");
                string byteCode = fe.Compile(input);

                
                if (! listener.HasErrors() && byteCode != null)
                {
                    input = new ANTLRStringStream(byteCode);
                    ByteCompilerFrontend bfe = new ByteCompilerFrontend();
                    bfe.TraceDestination = redirector;
                    bfe.Listener = listener;
                    bfe.Compile(input);

                    CompiledScript script = bfe.Result;
                    Console.WriteLine("** usage info **");
                    if (script != null) Console.WriteLine("Base memory: {0} bytes", script.CalcBaseMemorySize());
                    //SaveScript(script);

                    /*
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
                    }
                     * */
                }

                
            }
        }

        private static void SaveScript(CompiledScript script)
        {
            SerializedScript serScript = SerializedScript.FromCompiledScript(script);

            using (var file = File.Create("script.plx"))
            {
                ProtoBuf.Serializer.Serialize(file, serScript);
            }

            using (var file = File.OpenRead("script.plx"))
            {
                SerializedScript readscript = ProtoBuf.Serializer.Deserialize<SerializedScript>(file);
                CompiledScript compiled = readscript.ToCompiledScript();
            }

            
        }
    }
}
