using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;

namespace InWorldz.Phlox.Compiler
{
    public class LSLTreeAdaptor : CommonTreeAdaptor
    {
        public override object Create(Antlr.Runtime.IToken payload)
        {
            return new LSLAst(payload);
        }

        public override object Create(Antlr.Runtime.IToken fromToken, string text)
        {
            fromToken = CreateToken(fromToken);
            fromToken.Text = text;
            return new LSLAst(fromToken);
        }

        public override object Create(int tokenType, Antlr.Runtime.IToken fromToken)
        {
            fromToken = CreateToken(fromToken);
            fromToken.Type = tokenType;
            return new LSLAst(fromToken);
        }
        
        public override object Create(int tokenType, Antlr.Runtime.IToken fromToken, string text)
        {
            fromToken = CreateToken(fromToken);
            fromToken.Type = tokenType;
            fromToken.Text = text;
            return new LSLAst(fromToken);
        }

        public override object Create(int tokenType, string text)
        {
            Antlr.Runtime.IToken token = CreateToken(tokenType, text);
            return new LSLAst(token);
        }

        public override object DupNode(object t)
        {
            if (t == null) return null;

            return this.Create(((LSLAst)t).Token); 
        }

        public override object ErrorNode(Antlr.Runtime.ITokenStream input, Antlr.Runtime.IToken start, Antlr.Runtime.IToken stop, Antlr.Runtime.RecognitionException e)
        {
            return new LSLErrorNode(input, start, stop, e);
        }

    }
}
