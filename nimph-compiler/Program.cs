using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nimph_compiler
{
    class Program
    {
        static void Main(string[] args)
        {

            NimphLexer lexer = new NimphLexer();

            string fileContents;
            using (StreamReader streamReader = new StreamReader(args[0], Encoding.UTF8))
            {
                fileContents = streamReader.ReadToEnd();
            }

            List<CharDFA.Token> tokens = lexer.LexString(args[0], fileContents);
            
            foreach (CharDFA.Token token in tokens)
            {
                Console.WriteLine(token.name + ", " + token.value);
            }

            NimphParser parser = new NimphParser();
            TokenSA.Node ast = parser.TokenStreamToAST(tokens);

            // Create the VM
            // Pass ast to VM

            Console.WriteLine("Press any key to end.");
            Console.ReadKey();

        }
    }
}
