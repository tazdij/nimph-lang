using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nimph_compiler
{
    /* Token Stack Automaton */
    class TokenSA
    {

        public class Node
        {
            public int? node_type;
            public List<Node> children;
            public CharDFA.Token? token;

            public Node()
            {
                children = new List<Node>();
                node_type = null;
                token = null;
            }
        }

        public class TokenStreamer
        {
            public List<CharDFA.Token> _tokens { private set; get; }
            public int _pos { private set; get; }

            public TokenStreamer(List<CharDFA.Token> tokens)
            {
                _tokens = tokens;
                _pos = 0;
            }

            public CharDFA.Token? Next()
            {
                if (_pos < _tokens.Count)
                    _pos++;
                else
                    return null;

                return _tokens[_pos - 1];
            }

            public CharDFA.Token? Previous()
            {
                if (_pos > 0)
                    _pos--;
                else
                    return null;

                return _tokens[_pos + 1];
            }

            public CharDFA.Token? Peek()
            {
                if (_pos < _tokens.Count)
                    return _tokens[_pos];

                return null;
            }

            public CharDFA.Token? Recall()
            {
                if (_pos > 0)
                    return _tokens[_pos - 1];

                return null;
            }

        }

        
        private List<Node> _stack;
        private Node _root = null;
        private TokenStreamer _stream;

        private void PushNode(Node node)
        {
            _stack.Add(node);
        }

        private void PopNode()
        {
            _stack.RemoveAt(_stack.Count - 1);
        }

        public TokenSA()
        {
            _stack = new List<Node>();

        }

        public Node ProcessTokens(List<CharDFA.Token> tokens)
        {
            _stream = new TokenStreamer(tokens);

            // Create the root node
            _root = new Node();

            PushNode(_root);
            Process(_root);
            PopNode();
            
            return _root;
        }

        private void Process(Node parent)
        {

            while (_stream.Peek().HasValue)
            {
                CharDFA.Token? token = _stream.Next();


                // return if stream has ended
                if (token == null) break;

                Node node = new Node();
                node.token = token.Value;

                // Check if we need to recurse
                if (token.Value.name == "LPAREN" || token.Value.name == "LBRACKET" || token.Value.name == "LBRACE")
                {
                    // Recurse if opening command
                    PushNode(node);
                    Process(node);
                    PopNode();
                }
                else if (token.Value.name == "RPAREN" || token.Value.name == "RBRACKET" || token.Value.name == "RBRACE")
                {
                    break;
                }

                parent.children.Add(node);
            }

            
        }
    }
}
