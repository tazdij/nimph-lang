using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nimph_compiler
{
    public class NimphVM
    {


        public class Scope
        {
            public Dictionary<String, String> symbols;
        }

        public List<Scope> _Symbols;

        public NimphVM()
        {
            _Symbols = new List<Scope>();
        }

    }
}
