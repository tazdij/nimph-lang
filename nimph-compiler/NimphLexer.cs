using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nimph_compiler
{
    class NimphLexer
    {

        private CharDFA cdfa;

        public NimphLexer()
        {
            this.cdfa = new CharDFA();

            CharDFA.State sta_start = cdfa.NewState("start");

            // Ignore whitespace
            sta_start.NewDelta("white_space", "[\\r\\n\\t\\ \x03]", sta_start, false, true);

            // Parse Subtract, can turn into a negative number if followed by any digit
            CharDFA.State sta_subtract = cdfa.NewState("subtract", "SUBTRACT");
            sta_start.NewDelta("start_to_subtract", "[\\-]", sta_subtract);

            // Parse Int, can start from a dash (SUBTRACT Token)
            //  Int can transition into Float
            CharDFA.State sta_int = cdfa.NewState("int");
            CharDFA.State sta_int_end = cdfa.NewState("end_int", "INT_B10");

            CharDFA.State sta_int_zero = cdfa.NewState("int_zero");
            CharDFA.State sta_int_bin = cdfa.NewState("int_bin");
            CharDFA.State sta_int_bin_end = cdfa.NewState("int_bin_end", "INT_B2");
            CharDFA.State sta_int_oct = cdfa.NewState("int_oct");
            CharDFA.State sta_int_oct_end = cdfa.NewState("int_oct_end", "INT_B8");
            CharDFA.State sta_int_hex = cdfa.NewState("int_hex");
            CharDFA.State sta_int_hex_end = cdfa.NewState("int_hex_end", "INT_B16");

            sta_start.NewDelta("start_to_int", "[1-9]", sta_int);
            sta_start.NewDelta("start_to_int_zero", "[0]", sta_int_zero);
            sta_subtract.NewDelta("subtract_to_int", "[0-9]", sta_int);

            sta_int.NewDelta("int_to_int", "[0-9]", sta_int);
            sta_int.NewDelta("int_to_end", "[^0-9\\.]", sta_int_end, false, false);

            sta_int_zero.NewDelta("int_zero_to_int_bin", "[b]", sta_int_bin);
            sta_int_zero.NewDelta("int_zero_to_int_oct", "[o]", sta_int_oct);
            sta_int_zero.NewDelta("int_zero_to_int_hex", "[x]", sta_int_hex);
            sta_int_zero.NewDelta("int_zero_to_int_end", "[^box]", sta_int_end, false, false);

            sta_int_bin.NewDelta("int_bin_to_int_bin", "[01]", sta_int_bin);
            sta_int_bin.NewDelta("int_bin_to_int_bin_end", "[^01]", sta_int_bin_end);

            sta_int_oct.NewDelta("int_oct_to_int_oct", "[0-7]", sta_int_oct);
            sta_int_oct.NewDelta("int_oct_to_int_oct_end", "[^0-7]", sta_int_oct_end);

            sta_int_hex.NewDelta("int_hex_to_int_hex", "[0-9A-F]", sta_int_hex);
            sta_int_hex.NewDelta("int_hex_to_int_hex_end", "[^0-9A-F]", sta_int_hex_end);


            // Parse Float
            // Floats are only started from an int, once a decimal point is found
            CharDFA.State sta_float = cdfa.NewState("float");
            CharDFA.State sta_float_end = cdfa.NewState("end_float", "FLOAT");
            sta_int.NewDelta("int_to_float", "[\\.]", sta_float);
            sta_float.NewDelta("float_to_float", "[0-9]", sta_float);
            sta_float.NewDelta("float_to_end", "[^0-9]", sta_float_end, false, false);


            // Parse symbol
            CharDFA.State sta_symbol_start = cdfa.NewState("symbol_start");
            CharDFA.State sta_symbol_chars = cdfa.NewState("symbol_chars");
            CharDFA.State sta_symbol_end = cdfa.NewState("symbol_end", "SYMBOL");
            sta_start.NewDelta("start_to_symbol", "[a-zA-Z_\\+\\-\\/]", sta_symbol_start);
            sta_symbol_start.NewDelta("symbol_start_to_symbol_chars", "[a-zA-Z_\\+\\-0-9\\/\\.]", sta_symbol_chars);
            sta_symbol_chars.NewDelta("symbol_chars_to_symbol_chars", "[a-zA-Z_\\+\\-0-9\\/\\.]", sta_symbol_chars);
            sta_symbol_chars.NewDelta("symbol_chars_to_symbol_end", "[^a-zA-Z_\\+\\-0-9\\/\\.]", sta_symbol_end, false, false);
            sta_symbol_start.NewDelta("symbol_chars_to_symbol_end", "[^a-zA-Z_\\+\\-0-9\\/\\.]", sta_symbol_end, false, false);

            // Parse string
            CharDFA.State sta_string_dq_start = cdfa.NewState("string_dq_start");
            CharDFA.State sta_string_dq_chars = cdfa.NewState("string_dq_chars");
            CharDFA.State sta_string_dq_escape = cdfa.NewState("string_dq_escape");
            CharDFA.State sta_string_dq_end = cdfa.NewState("string_dq_end", "STRING");

            sta_start.NewDelta("start_to_string_dq_start", "[\\\"]", sta_string_dq_start, false);
            sta_string_dq_start.NewDelta("string_dq_start_to_string_dq_chars", "[^\\\"\\\\]", sta_string_dq_chars);
            sta_string_dq_start.NewDelta("string_dq_start_to_string_dq_escape", "[\\\\]", sta_string_dq_escape);
            sta_string_dq_start.NewDelta("string_dq_start_to_string_dq_end", "[\\\"]", sta_string_dq_end, false);
            sta_string_dq_chars.NewDelta("string_dq_chars_to_string_dq_chars", "[^\\\"\\\\]", sta_string_dq_chars);
            sta_string_dq_chars.NewDelta("string_dq_chars_to_string_dq_escape", "[\\\\]", sta_string_dq_escape);
            sta_string_dq_chars.NewDelta("string_dq_chars_to_string_dq_end", "[\\\"]", sta_string_dq_end, false);
            sta_string_dq_escape.NewDelta("string_dq_escapte_to_string_dq_chars", ".", sta_string_dq_chars);

            CharDFA.State sta_string_sq_start = cdfa.NewState("string_sq_start");
            CharDFA.State sta_string_sq_chars = cdfa.NewState("string_sq_chars");
            CharDFA.State sta_string_sq_escape = cdfa.NewState("string_sq_escape");
            CharDFA.State sta_string_sq_end = cdfa.NewState("string_sq_end", "STRING");

            sta_start.NewDelta("start_to_string_sq_start", "[\\']", sta_string_sq_start, false);
            sta_string_sq_start.NewDelta("string_sq_start_to_string_sq_chars", "[^\\'\\\\]", sta_string_sq_chars);
            sta_string_sq_start.NewDelta("string_sq_start_to_string_sq_escape", "[\\\\]", sta_string_sq_escape);
            sta_string_sq_start.NewDelta("string_sq_start_to_string_sq_end", "[\\']", sta_string_sq_end, false);
            sta_string_sq_chars.NewDelta("string_sq_chars_to_string_sq_chars", "[^\\'\\\\]", sta_string_sq_chars);
            sta_string_sq_chars.NewDelta("string_sq_chars_to_string_sq_escape", "[\\\\]", sta_string_sq_escape);
            sta_string_sq_chars.NewDelta("string_sq_chars_to_string_sq_end", "[\\']", sta_string_sq_end, false);
            sta_string_sq_escape.NewDelta("string_sq_escapte_to_string_sq_chars", ".", sta_string_sq_chars);

            // Parse Left Brace
            CharDFA.State sta_lbrace = cdfa.NewState("lbrace", "LBRACE");
            sta_start.NewDelta("start_to_lbrace", "[\\{]", sta_lbrace);

            // Parse Right Brace
            CharDFA.State sta_rbrace = cdfa.NewState("rbrace", "RBRACE");
            sta_start.NewDelta("start_to_rbrace", "[\\}]", sta_rbrace);

            // Parse Left Bracket
            CharDFA.State sta_lbracket = cdfa.NewState("lbracket", "LBRACKET");
            sta_start.NewDelta("start_to_lbracket", "[\\[]", sta_lbracket);

            // Parse Right Bracket
            CharDFA.State sta_rbracket = cdfa.NewState("rbracket", "RBRACKET");
            sta_start.NewDelta("start_to_rbracket", "[\\]]", sta_rbracket);

            // Parse Left Brace
            CharDFA.State sta_lparen = cdfa.NewState("lparen", "LPAREN");
            sta_start.NewDelta("start_to_lparen", "[\\(]", sta_lparen);

            // Parse codegen tick
            CharDFA.State sta_tick = cdfa.NewState("tick", "TICK");
            sta_start.NewDelta("start_to_tick", "[\\`]", sta_tick);

            // Parse Right Brace
            CharDFA.State sta_rparen = cdfa.NewState("rparen", "RPAREN");
            sta_start.NewDelta("start_to_rparen", "[\\)]", sta_rparen);
        }

        public List<CharDFA.Token> LexString(string filename, string source)
        {
            List<CharDFA.Token> tokens = cdfa.ProcessFile(filename, source);
            return tokens;
        }

    }
}
