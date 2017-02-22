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

        private TokenDFA tdfa;

        public NimphParser()
        {
            TokenDFA.Comparator cmp = new TokenDFA.Comparator(TokenDFA.Comparator.Polarity.IsIn, new List<string> { "LPAREN" });
        }

        public void TokenStreamToAST(List<CharDFA.Token> stream)
        {



        }


    }
}
