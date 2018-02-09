using System;
using Environment = Aurora_Language.Obj.Environment;

namespace Aurora_Language
{
    public static class Repl
    {
        private const string PROMPT = "Aurora: ";

        public static void Start()
        {
            var enviroment = new Environment();

            while (true)
            {
                Console.Write(PROMPT);

                var input = Console.ReadLine();

                if (input == ".exit") return;

                // TODO: Fix issue with infinite loop if semicolon is missing.

                var lexer = new Lexer(input);
                var parser = new Parser(lexer);

                var program = parser.ParseProgram();

                var evaluated = Evaluator.Evaluate(program, enviroment);

                while (evaluated != null)
                {
                    Console.WriteLine("{0}", evaluated.Inspect());
                    break;
                }

                Console.WriteLine(" ");
            }
        }
    }
}