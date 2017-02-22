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

            public Delta(string name, string pattern, State to_state) : this(name, pattern, to_state, true, true)
            { }

            public Delta(string name, Comparator comp, State to_state, bool collect, bool advance)
            {
                this.name = name;
                comparator = comp;
                this.to_state = to_state;
            }

            public bool IsMatch(char curChar)
            {
                if (comparator.IsMatch(curChar.ToString()))
                {
                    return true;
                }
                return false;
            }
        }

        public class State
        {

        }
    }
}
