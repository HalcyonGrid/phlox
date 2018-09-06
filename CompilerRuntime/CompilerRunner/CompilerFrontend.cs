using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Halcyon.Phlox.Compiler;
using Halcyon.Phlox.Types;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Antlr3.ST;
using System.IO;
using Halcyon.Phlox.Compiler.BranchAnalyze;

namespace CompilerRunner
{
    class CompilerFrontend
    {
        private System.IO.TextWriter _traceDestination;
        public System.IO.TextWriter TraceDestination
        {
            get
            {
                return _traceDestination;
            }

            set
            {
                _traceDestination = value;
            }
        }

        private ILSLListener _listener;
        public ILSLListener Listener
        {
            get
            {
                return _listener;
            }

            set
            {
                _listener = value;
            }
        }

        public string Compile(ICharStream input)
        {
            try
            {
                LSLTreeAdaptor lslAdaptor = new LSLTreeAdaptor();

                LSLLexer lex = new LSLLexer(input);
                CommonTokenStream tokens = new CommonTokenStream(lex);
                LSLParser p = new LSLParser(tokens);
                p.TreeAdaptor = lslAdaptor;
                p.TraceDestination = _traceDestination;
                lex.TraceDestination = _traceDestination;
                LSLParser.prog_return r = p.prog();

                if (p.NumberOfSyntaxErrors > 0)
                {
                    if (_listener != null)
                    {
                        _listener.Error(Convert.ToString(p.NumberOfSyntaxErrors) + " syntax error(s)");
                    }

                    return null;
                }

                CommonTree t = (CommonTree)r.Tree;
                CommonTreeNodeStream nodes = new CommonTreeNodeStream(lslAdaptor, t);
                nodes.TokenStream = tokens;

                SymbolTable symtab = new SymbolTable(tokens, Defaults.SystemMethods.Values, DefaultConstants.Constants.Values);

                if (this.Listener != null)
                {
                    symtab.StatusListener = this.Listener;
                }

                Def def = new Def(nodes, symtab);
                def.TraceDestination = _traceDestination;
                def.Downup(t);

                CommonTreeNodeStream nodes2 = new CommonTreeNodeStream(lslAdaptor, t);
                nodes2.TokenStream = tokens;

                Types types = new Types(nodes2, symtab);
                types.TraceDestination = _traceDestination;
                types.Downup(t);

                if (_listener != null)
                {
                    if (_listener.HasErrors()) return null;
                }

                CommonTreeNodeStream nodes4 = new CommonTreeNodeStream(lslAdaptor, t);
                nodes4.TokenStream = tokens;

                CommonTreeNodeStream nodes3 = new CommonTreeNodeStream(lslAdaptor, t);
                nodes3.TokenStream = tokens;

                TextReader fr = new StreamReader("ByteCode.stg");
                StringTemplateGroup templates = new StringTemplateGroup(fr);
                fr.Close();

                DotTreeGenerator dotgen = new DotTreeGenerator();
                string dot = dotgen.ToDot(t);

                TextWriter tw = new StreamWriter("ast.txt");
                tw.WriteLine(dot);
                tw.Close();

                Analyze analyze = new Analyze(nodes4, symtab);
                types.TraceDestination = _traceDestination;
                analyze.Downup(t);

                foreach (FunctionBranch b in analyze.FunctionBranches.Where(pred => pred.Type != null))
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

                Gen g = new Gen(nodes3, symtab);
                g.TemplateGroup = templates;
                g.TraceDestination = _traceDestination;
                StringTemplate template = g.script().Template;


                if (template != null)
                {
                    string bcOut = template.ToString();
                    Console.WriteLine("** byte code **\n" + bcOut);
                    return bcOut;
                }
            }
            catch (TooManyErrorsException e)
            {
                if (_listener != null)
                {
                    _listener.Error(String.Format("Too many errors {0}", e.InnerException.Message));
                }
            }
            catch (RecognitionException e)
            {
                if (_listener != null)
                {
                    _listener.Error("line: " + e.Line.ToString() + ":" + e.CharPositionInLine.ToString() + " " + e.Message);
                }
            }
            catch (Exception e)
            {
                if (_listener != null)
                {
                    _listener.Error(e.Message);
                }
            }

            return null;
        }
    }
}
