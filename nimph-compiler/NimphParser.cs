using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nimph_compiler
{
    class NimphParser
    {

        public enum NodeType {
            Expr,
            QExpr,
            LExpr,
            Args,
            Symbol,
            Literal
        }

        private TokenSA tsa;

        public NimphParser()
        {
            tsa = new TokenSA();
            //TokenSA.Comparator cmp = new TokenSA.Comparator(TokenSA.Comparator.Polarity.IsIn, new List<string> { "LPAREN" });
        }

        public TokenSA.Node TokenStreamToAST(List<CharDFA.Token> stream)
        {
            TokenSA.Node ast = tsa.ProcessTokens(stream);

            return ast;
        }


    }
}
