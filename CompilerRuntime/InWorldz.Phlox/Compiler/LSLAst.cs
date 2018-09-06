using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using Antlr.Runtime;

namespace Halcyon.Phlox.Compiler
{
    public class LSLAst : CommonTree
    {
        public IScope scope;
        public Symbol symbol;
        public ISymbolType evalType;
        public ISymbolType promoteToType;

        public LSLAst()
        {
        }

        public LSLAst(IToken t)
            : base(t)
        {
        }
    }
}
