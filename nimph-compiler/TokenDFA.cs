using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nimph_compiler
{
    class TokenDFA
    {

        public class Node
        {
            int node_type;
            List<Node> children;
            CharDFA.Token token;
        }

        public class Comparator
        {
            public enum Polarity
            {
                IsIn,
                IsNotIn
            }
            public List<string> options { private set; get; }
            public Polarity polarity { private set; get; }

            public Comparator(Polarity polarity, List<string> options)
            {
                this.options = options;
                this.polarity = polarity;
            }

            public bool IsMatch(string val)
            {
                if (polarity == Polarity.IsIn)
                {
                    if (options.Contains(val)) return true;
                }
                else
                {
                    if (!options.Contains(val)) return true;
                }
                return false;
            }
        }

        public class Delta
        {
            public Comparator comparator { private set; get; }
            string name = null;
            public State to_state { private set; get; }

            
            public Delta(string name, Comparator comp, State to_state)
            {
                this.name = name;
                comparator = comp;
                this.to_state = to_state;
            }

            public bool IsMatch(string ident)
            {
                if (comparator.IsMatch(ident))
                {
                    return true;
                }
                return false;
            }
        }

        public class State
        {
            List<Delta> _deltas;
            string _name;
            public string _tok_id { private set; get; }


            public State(string name, string tok_id)
            {
                _deltas = new List<Delta>();
                _name = name;
                _tok_id = tok_id;
            }

            public Delta NewDelta(string name, Comparator comp, State to_state)
            {
                Delta delta = new Delta(name, comp, to_state);
                _deltas.Add(delta);
                return delta;
            }

            public string AcceptOptions()
            {
                string accepts = "";
                foreach (Delta delta in _deltas)
                {
                    accepts += delta.comparator.ToString() + ", ";
                }

                return accepts;
            }

            public bool IsLeaf()
            {
                if (_deltas.Count == 0)
                    return true;

                return false;
            }


            public bool IsValidToken(string ident)
            {
                foreach (Delta delta in _deltas)
                {
                    if (delta.IsMatch(character))
                    {
                        return true;
                    }
                }

                return false;
            }

            public char? AcceptToken(CharDFA.Token character, out State next_state, out bool advance)
            {
                next_state = null;
                advance = false;
                foreach (Delta delta in _deltas)
                {
                    if (delta.IsMatch(character))
                    {
                        next_state = delta.to_state;

                        advance = delta.advance_char;

                        if (!delta.collect_char)
                            return null;

                        return character;
                    }
                }

                return null;
            }
        }
        

        private Dictionary<string, State> _states;
        private State _activeState = null;
        private State _startState = null;
    }
}
