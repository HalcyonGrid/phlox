using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InWorldz.Phlox.Types;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using InWorldz.Phlox.Compiler;
using InWorldz.Phlox.ByteCompiler;
using Antlr3.ST;
using System.IO;

namespace InWorldz.Phlox.Glue
{
    /// <summary>
    /// Provides a frontend to compile input
    /// </summary>
    public class CompilerFrontend
    {
        private ILSLListener _listener;
        private LSLListenerTraceRedirector _traceRedirect;
        private string _templatePath;
        private bool _byteCodeDebugging = false;
        private string _byteCode;
        private bool _outputAstGraph = false;

        /// <summary>
        /// Whether or not to write a DOT file containing the source code AST
        /// </summary>
        public bool OutputASTGraph
        {
            get
            {
                return _outputAstGraph;
            }

            set
            {
                _outputAstGraph = value;
            }
        }

        public ILSLListener Listener
        {
            get
            {
                return _listener;
            }
        }

        public string GeneratedByteCode
        {
            get
            {
                return _byteCode;
            }
        }

        public CompilerFrontend(ILSLListener listener, string templatePath)
        {
            _listener = listener;
            _traceRedirect = new LSLListenerTraceRedirector(listener);
            _templatePath = templatePath;
        }

        public CompilerFrontend(ILSLListener listener, string templatePath, bool byteCodeDebugging)
        {
            _listener = listener;
            _traceRedirect = new LSLListenerTraceRedirector(listener);
            _templatePath = templatePath;
            _byteCodeDebugging = byteCodeDebugging;
        }

        public VM.CompiledScript Compile(string input)
        {
            return this.Compile(new ANTLRStringStream(input));
        }

        public VM.CompiledScript Compile(ICharStream input)
        {
            try
            {
                LSLTreeAdaptor lslAdaptor = new LSLTreeAdaptor();


                //
                // Initial parse and AST creation
                //
                LSLLexer lex = new LSLLexer(input);
                CommonTokenStream tokens = new CommonTokenStream(lex);
                LSLParser p = new LSLParser(tokens);
                p.TreeAdaptor = lslAdaptor;
                p.TraceDestination = _traceRedirect;
                lex.TraceDestination = _traceRedirect;
                LSLParser.prog_return r = p.prog();

                if (p.NumberOfSyntaxErrors > 0)
                {
                    _listener.Error(Convert.ToString(p.NumberOfSyntaxErrors) + " syntax error(s)");
                    return null;
                }


                //
                // Definitions
                //
                CommonTree t = (CommonTree)r.Tree;
                CommonTreeNodeStream nodes = new CommonTreeNodeStream(lslAdaptor, t);
                nodes.TokenStream = tokens;

                SymbolTable symtab = new SymbolTable(tokens, Defaults.SystemMethods.Values, DefaultConstants.Constants.Values);
                symtab.StatusListener = _listener;

                Def def = new Def(nodes, symtab);
                def.TraceDestination = _traceRedirect;
                def.Downup(t);

                nodes.Reset();

                if (_listener.HasErrors() || def.NumberOfSyntaxErrors > 0)
                {
                    return null;
                }


                //
                // Type and more semantic checks
                //
                Compiler.Types types = new Compiler.Types(nodes, symtab);
                types.TraceDestination = _traceRedirect;
                types.Downup(t);

                nodes.Reset();

                if (_listener.HasErrors() || types.NumberOfSyntaxErrors > 0)
                {
                    return null;
                }


                StringTemplateGroup templates;
                using (TextReader fr = new StreamReader(Path.Combine(_templatePath, "ByteCode.stg")))
                {
                    templates = new StringTemplateGroup(fr);
                    fr.Close();
                }

                if (_outputAstGraph)
                {
                    DotTreeGenerator dotgen = new DotTreeGenerator();
                    string dot = dotgen.ToDot(t);

                    TextWriter tw = new StreamWriter("ast.txt");
                    tw.WriteLine(dot);
                    tw.Close();
                }

                Analyze analyze = new Analyze(nodes, symtab);
                analyze.TraceDestination = _traceRedirect;
                analyze.Downup(t);

                nodes.Reset();

                foreach (Compiler.BranchAnalyze.FunctionBranch b in analyze.FunctionBranches.Where(pred => pred.Type != null))
                {
                    if (!b.AllCodePathsReturn())
                    {
                        if (_listener != null)
                        {
                            _listener.Error("line: " + b.Node.Line + ":" +
                                b.Node.CharPositionInLine + " " + b.Node.Text + "(): Not all control paths return a value");
                        }
                    }
                }

                if (_listener.HasErrors() || analyze.NumberOfSyntaxErrors > 0)
                {
                    return null;
                }


                //
                // Bytecode generation
                //
                Gen g = new Gen(nodes, symtab);
                g.TemplateGroup = templates;
                g.TraceDestination = _traceRedirect;

                Gen.script_return ret = g.script();

                if (_listener.HasErrors() || g.NumberOfSyntaxErrors > 0)
                {
                    return null;
                }

                if (ret.Template == null)
                {
                    return null;
                }

                StringTemplate template = ret.Template;

                if (_byteCodeDebugging)
                {
                    _byteCode = template.ToString();
                }

                //
                // Bytecode compilation
                //
                AssemblerLexer alex = new AssemblerLexer(new ANTLRStringStream(template.ToString()));
                CommonTokenStream atokens = new CommonTokenStream(alex);
                AssemblerParser ap = new AssemblerParser(atokens);
                BytecodeGenerator bcgen = new BytecodeGenerator(Defaults.SystemMethods.Values);

                ap.SetGenerator(bcgen);
                ap.TraceDestination = _traceRedirect;

                try
                {
                    ap.program();

                    if (_listener.HasErrors() || p.NumberOfSyntaxErrors > 0)
                    {
                        _listener.Error(Convert.ToString(ap.NumberOfSyntaxErrors) + " bytecode generation error(s)");
                        return null;
                    }

                    return bcgen.Result;
                }
                catch (GenerationException e)
                {
                    _listener.Error(e.Message);
                }

                return null;
            }
            catch (TooManyErrorsException e)
            {
                _listener.Error(String.Format("Too many errors {0}", e.InnerException.Message));
            }
            catch (RecognitionException e)
            {
                _listener.Error("line: " + e.Line.ToString() + ":" + e.CharPositionInLine.ToString() + " " + e.Message);
            }
            catch (Exception e)
            {
                _listener.Error(e.Message);    
            }

            return null;
        }
    }
}
