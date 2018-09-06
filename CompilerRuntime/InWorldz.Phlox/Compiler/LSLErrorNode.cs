using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Antlr.Runtime;
using Antlr.Runtime.Tree;


namespace Halcyon.Phlox.Compiler
{
    public class LSLErrorNode : LSLAst
    {
        private CommonErrorNode _delegate;

        public LSLErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
        {
            _delegate = new CommonErrorNode(input, start, stop, e);
        }

        public override bool IsNil
        {
            get
            {
                return _delegate.IsNil;
            }
        }

        public override int Type
        {
            get
            {
                return _delegate.Type;
            }
        }

        public override string Text
        {
            get
            {
                return _delegate.Text;
            }
        }

        public override string ToString()
        {
            return _delegate.ToString();
        }
    }
}
