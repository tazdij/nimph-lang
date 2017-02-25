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

            tsa.RegisterHandler("LPAREN", HandleLPARENTokens);
            tsa.RegisterHandler("LBRACKET", HandleLBRACKETTokens);
            tsa.RegisterHandler("LBRACE", HandleLBRACETokens);
            tsa.RegisterHandler("TICK", HandleTICKToken);
        }

        

        public TokenSA.Node TokenStreamToAST(List<CharDFA.Token> stream)
        {
            TokenSA.Node ast = tsa.ProcessTokens(stream);

            return ast;
        }

        protected bool HandleTICKToken(TokenSA.Node parent, TokenSA.TokenStreamer stream)
        {
            TokenSA.Node node;

            // Check for a LParen
            CharDFA.Token? token = stream.Peek();
            if (!token.HasValue) return false;

            if (token.Value.name != "LPAREN") return false;

            node = tsa.ProcessNext();

            foreach (TokenSA.Node child in node.children)
            {
                parent.children.Add(child);
            }

            return false;
        }


        protected bool HandleLPARENTokens(TokenSA.Node parent, TokenSA.TokenStreamer stream)
        {
            TokenSA.Comparator allowedChildren = new TokenSA.Comparator(TokenSA.Comparator.Direction.IsIn, new string[] { "SYMBOL" });

            TokenSA.Node cur_node = null;

            while (true)
            {
                cur_node = tsa.ProcessNext();
                if (cur_node == null)
                    break;

                if (cur_node.token.Value.name == "RPAREN")
                    break;

                parent.children.Add(cur_node);
            }


            return false;
        }

        protected bool HandleLBRACKETTokens(TokenSA.Node parent, TokenSA.TokenStreamer stream)
        {
            TokenSA.Comparator allowedChildren = new TokenSA.Comparator(TokenSA.Comparator.Direction.IsIn, new string[] { "SYMBOL" });

            TokenSA.Node cur_node = null;

            while (true)
            {
                cur_node = tsa.ProcessNext();
                if (cur_node == null)
                    break;

                if (cur_node.token.Value.name == "RBRACKET")
                    break;

                parent.children.Add(cur_node);
            }


            return false;
        }

        protected bool HandleLBRACETokens(TokenSA.Node parent, TokenSA.TokenStreamer stream)
        {
            TokenSA.Comparator allowedChildren = new TokenSA.Comparator(TokenSA.Comparator.Direction.IsIn, new string[] { "SYMBOL" });

            TokenSA.Node cur_node = null;

            while (true)
            {
                cur_node = tsa.ProcessNext();
                if (cur_node == null)
                    break;

                if (cur_node.token.Value.name == "RBRACE")
                    break;

                parent.children.Add(cur_node);
            }


            return false;
        }


    }
}
