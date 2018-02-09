using System;
using System.Collections.Generic;
using Aurora_Language;
using Aurora_Language.Data;
using Xunit;

// TODO: Rafactor tests.

namespace Test_Aurora_Language
{
    public class Test_Parser
    {
        private struct IdentifierStruct
        {
            public readonly string Identifier;

            public IdentifierStruct(string identifier)
            {
                Identifier = identifier;
            }
        }

        private struct PrefixStruct
        {
            public readonly string Input;
            public readonly string Operator;
            public readonly long IntegerValue;

            public PrefixStruct(string input, string @operator, long integerValue)
            {
                Input = input;
                Operator = @operator;
                IntegerValue = integerValue;
            }
        }

        private struct InfixStruct
        {
            public readonly string Input;
            public readonly long LeftValue;
            public readonly string Operator;
            public readonly long RightValue;

            public InfixStruct(string input, long leftValue, string @operator, long rightValue)
            {
                Input = input;
                LeftValue = leftValue;
                Operator = @operator;
                RightValue = rightValue;
            }
        }

        private struct FunctionParametersStruct
        {
            public readonly string Input;
            public readonly List<string> ExpectedParams;

            public FunctionParametersStruct(string input, List<string> expectedParams)
            {
                Input = input;
                ExpectedParams = expectedParams;
            }
        }

        private static bool TestLetStatement(IStatement statement, string name)
        {
            if (statement.TokenLiteral() != "let")
            {
                Console.WriteLine("Expected 'len', but got {0}", statement.TokenLiteral());
                return false;
            }

            var letStatement = statement.GetType();

            if (letStatement != typeof(LetStatement))
            {
                Console.WriteLine("Expected 'LetStatement', but got {0}", letStatement);
                return false;
            }

            if (statement.Name.StringValue != name)
            {
                Console.WriteLine("Expected {0}, but got {1}", statement.Name.StringValue, name);
                return false;
            }

            if (statement.Name.TokenLiteral() != name)
            {
                Console.WriteLine("Expected {0}, but got {1}", statement.Name, name);
                return false;
            }

            return true;
        }

        private bool TestIntegerLiteral(IExpression literal, long value)
        {
            var integer = literal.GetType();

            if (integer != typeof(IntegerLiteral)) return false;

            if (literal.LongValue != value) return false;

            return true;
        }

        private void checkParserErrors(Parser parser)
        {
            var errors = parser.Errors();

            var title = $"Parser found {errors.Count} errors.";

            if (errors.Count > 0)
                foreach (var error in errors)
                    throw new Exception(title + "   " + error);
        }

        [Fact]
        public void TestBooleanExpression()
        {
            const string input = "true;";

            var lexer = new Lexer(input);

            var parser = new Parser(lexer);

            var program = parser.ParseProgram();
            checkParserErrors(parser);

            if (program == null) throw new Exception("Program is null.");

            if (program.Statements.Count != 1)
                throw new Exception(string.Format("{0} statements was founded, but expected 1",
                    program.Statements.Count.ToString()));

            var statement = program.Statements[0];

            if (statement.GetType() != typeof(ExpressionStatement))
                throw new Exception(string.Format("Expected 'ExpressionStatement', but got {0}", statement));

            var literal = statement.Expression;

            if (literal.GetType() != typeof(BooleanExpression))
                throw new Exception(string.Format("Expected 'BooleanExpression', but got {0}", literal));

            if (literal.BoolValue != true)
                throw new Exception(string.Format("Expected 'true', but got {0}", literal.BoolValue));

            if (literal.TokenLiteral() != "true")
                throw new Exception(string.Format("Expected 'true', but got {0}", literal.TokenLiteral()));
        }

        [Fact]
        public void TestCallExpressionParsing()
        {
            const string input = "add(1, 2 * 3, 4 + 5);";

            var lexer = new Lexer(input);

            var parser = new Parser(lexer);

            var program = parser.ParseProgram();
            checkParserErrors(parser);

            if (program == null) throw new Exception("Program is null.");

            if (program.Statements.Count != 1)
                throw new Exception(string.Format("{0} statements was founded, but expected 1",
                    program.Statements.Count.ToString()));

            var statement = program.Statements[0];

            if (statement.GetType() != typeof(ExpressionStatement))
                throw new Exception(string.Format("Expected 'ExpressionStatement', but got {0}", statement));

            var literal = statement.Expression;

            if (literal.GetType() != typeof(CallExpression))
                throw new Exception(string.Format("Expected 'CallExpression', but got {0}", literal));

            if (literal.Arguments.Count != 3)
                throw new Exception(string.Format("Expected '3', but got {0}", literal.Arguments.Count));
        }

        [Fact]
        public void TestFunctionLiteralParsing()
        {
            const string input = "fn(x, y) { x + y; }";

            var lexer = new Lexer(input);

            var parser = new Parser(lexer);

            var program = parser.ParseProgram();
            checkParserErrors(parser);

            if (program == null) throw new Exception("Program is null.");

            if (program.Statements.Count != 1)
                throw new Exception(string.Format("{0} statements was founded, but expected 1",
                    program.Statements.Count.ToString()));

            var statement = program.Statements[0];

            if (statement.GetType() != typeof(ExpressionStatement))
                throw new Exception(string.Format("Expected 'ExpressionStatement', but got {0}", statement));

            var literal = statement.Expression;

            if (literal.GetType() != typeof(FunctionLiteral))
                throw new Exception(string.Format("Expected 'FunctionLiteral', but got {0}", literal));

            if (literal.Parameters.Count != 2)
                throw new Exception(string.Format("Expected '2', but got {0}", literal.Parameters.Count));

            if (literal.Body.Statements.Count != 1)
                throw new Exception(string.Format("Expected '1', but got {0}", literal.Body.Statements.Count));
        }

        [Fact]
        public void TestFunctionParameterParsing()
        {
            var data = new FunctionParametersStruct[3];

            data[0] = new FunctionParametersStruct("fn() {};", new List<string>());
            data[1] = new FunctionParametersStruct("fn(x) {};", new List<string> {"x"});
            data[2] = new FunctionParametersStruct("fn(x, y, z) {};", new List<string> {"x", "y", "z"});


            foreach (var parametersStruct in data)
            {
                var lexer = new Lexer(parametersStruct.Input);

                var parser = new Parser(lexer);

                var program = parser.ParseProgram();
                checkParserErrors(parser);

                var statement = program.Statements[0];

                if (statement.GetType() != typeof(ExpressionStatement))
                    throw new Exception(string.Format("Expected 'ExpressionStatement', but got {0}", statement));

                var literal = statement.Expression;

                if (literal.GetType() != typeof(FunctionLiteral))
                    throw new Exception(string.Format("Expected 'FunctionLiteral', but got {0}", literal));

                if (literal.Parameters.Count != parametersStruct.ExpectedParams.Count)
                    throw new Exception(string.Format("Expected {1}, but got {0}", literal.Parameters.Count,
                        parametersStruct.ExpectedParams.Count));
            }
        }

        [Fact]
        public void TestIdentifierExpression()
        {
            const string input = "abc;";

            var lexer = new Lexer(input);

            var parser = new Parser(lexer);

            var program = parser.ParseProgram();
            checkParserErrors(parser);

            if (program == null) throw new Exception("Program is null.");

            if (program.Statements.Count != 1)
                throw new Exception(string.Format("{0} statements was founded, but expected 1",
                    program.Statements.Count.ToString()));

            var statement = program.Statements[0];

            if (statement.GetType() != typeof(ExpressionStatement))
                throw new Exception(string.Format("Expected 'ExpressionStatement', but got {0}", statement));

            var identifier = statement.Expression;

            if (identifier.GetType() != typeof(Identifier))
                throw new Exception(string.Format("Expected 'Identifier', but got {0}", identifier));

            if (identifier.StringValue != "abc")
                throw new Exception(string.Format("Expected 'abc', but got {0}", identifier.StringValue));

            if (identifier.TokenLiteral() != "abc")
                throw new Exception(string.Format("Expected 'abc', but got {0}", identifier.TokenLiteral()));
        }

        [Fact]
        public void TestIfExpression()
        {
            const string input = "if (x < y) { x }";

            var lexer = new Lexer(input);

            var parser = new Parser(lexer);

            var program = parser.ParseProgram();
            checkParserErrors(parser);

            if (program == null) throw new Exception("Program is null.");

            if (program.Statements.Count != 1)
                throw new Exception(string.Format("{0} statements was founded, but expected 1",
                    program.Statements.Count.ToString()));

            var statement = program.Statements[0];

            if (statement.GetType() != typeof(ExpressionStatement))
                throw new Exception(string.Format("Expected 'ExpressionStatement', but got {0}", statement));

            var literal = statement.Expression;

            if (literal.GetType() != typeof(IfExpression))
                throw new Exception(string.Format("Expected 'IfExpression', but got {0}", literal));

            if (literal.Consequence.Statements.Count != 1)
                throw new Exception(string.Format("Expected '1', but got {0}", literal.Consequence.Statements.Count));

            if (literal.Consequence.Statements[0].GetType() != typeof(ExpressionStatement))
                throw new Exception(string.Format("Expected 'ExpressionStatement', but got {0}",
                    literal.Consequence.Statements[0].GetType()));

            if (literal.Alternative != null)
                throw new Exception(string.Format("Expected 'null', but got {1}", literal.Alternative));
        }

        [Fact]
        public void TestIntegerLiteralExpression()
        {
            const string input = "5;";

            var lexer = new Lexer(input);

            var parser = new Parser(lexer);

            var program = parser.ParseProgram();
            checkParserErrors(parser);

            if (program == null) throw new Exception("Program is null.");

            if (program.Statements.Count != 1)
                throw new Exception(string.Format("{0} statements was founded, but expected 1",
                    program.Statements.Count.ToString()));

            var statement = program.Statements[0];

            if (statement.GetType() != typeof(ExpressionStatement))
                throw new Exception(string.Format("Expected 'ExpressionStatement', but got {0}", statement));

            var literal = statement.Expression;

            if (literal.GetType() != typeof(IntegerLiteral))
                throw new Exception(string.Format("Expected 'Identifier', but got {0}", literal));

            if (literal.LongValue != 5)
                throw new Exception(string.Format("Expected '5', but got {0}", literal.LongValue));

            if (literal.TokenLiteral() != "5")
                throw new Exception(string.Format("Expected '5', but got {0}", literal.TokenLiteral()));
        }

        [Fact]
        public void TestLetStatements()
        {
            const string input = @"
                    let x = 5;
                    let y = 7;
                    let abc = 2995;
                ";

            var lexer = new Lexer(input);

            var parser = new Parser(lexer);

            var program = parser.ParseProgram();
            checkParserErrors(parser);

            if (program == null) throw new Exception("Program is null.");

            if (program.Statements.Count != 3)
                throw new Exception(string.Format("{0} statements was founded, but expected 3",
                    program.Statements.Count.ToString()));

            var data = new IdentifierStruct[3];

            data[0] = new IdentifierStruct("x");
            data[1] = new IdentifierStruct("y");
            data[2] = new IdentifierStruct("abc");

            for (var i = 0; i < data.Length; i++)
                Assert.True(TestLetStatement(program.Statements[i], data[i].Identifier));
        }

        [Fact]
        public void TestParsingInfixExpressions()
        {
            var data = new InfixStruct[8];

            data[0] = new InfixStruct("5 + 5;", 5, "+", 5);
            data[1] = new InfixStruct("5 - 5;", 5, "-", 5);
            data[2] = new InfixStruct("5 * 5;", 5, "*", 5);
            data[3] = new InfixStruct("5 / 5;", 5, "/", 5);
            data[4] = new InfixStruct("5 > 5;", 5, ">", 5);
            data[5] = new InfixStruct("5 < 5;", 5, "<", 5);
            data[6] = new InfixStruct("5 == 5;", 5, "==", 5);
            data[7] = new InfixStruct("5 != 5;", 5, "!=", 5);

            foreach (var infixStruct in data)
            {
                var lexer = new Lexer(infixStruct.Input);

                var parser = new Parser(lexer);

                var program = parser.ParseProgram();
                checkParserErrors(parser);

                if (program == null) throw new Exception("Program is null.");

                if (program.Statements.Count != 1)
                    throw new Exception(string.Format("{0} statements was founded, but expected 1",
                        program.Statements.Count.ToString()));

                var statement = program.Statements[0];

                if (statement.GetType() != typeof(ExpressionStatement))
                    throw new Exception(string.Format("Expected 'ExpressionStatement', but got {0}", statement));

                var expression = statement.Expression;

                if (expression.GetType() != typeof(InfixExpression))
                    throw new Exception(string.Format("Expected 'InfixExpression', but got {0}", expression));

                if (expression.Operator != infixStruct.Operator)
                    throw new Exception(string.Format("Expected '{0}', but got {1}", infixStruct.Operator,
                        expression.Operator));

                Assert.True(TestIntegerLiteral(expression.Left, infixStruct.LeftValue));

                Assert.True(TestIntegerLiteral(expression.Right, infixStruct.RightValue));
            }
        }

        [Fact]
        public void TestParsingPrefixExpressions()
        {
            var data = new PrefixStruct[2];

            data[0] = new PrefixStruct("!5;", "!", 5);
            data[1] = new PrefixStruct("-7;", "-", 7);

            foreach (var prefixStruct in data)
            {
                var lexer = new Lexer(prefixStruct.Input);

                var parser = new Parser(lexer);

                var program = parser.ParseProgram();
                checkParserErrors(parser);

                if (program == null) throw new Exception("Program is null.");

                if (program.Statements.Count != 1)
                    throw new Exception(string.Format("{0} statements was founded, but expected 1",
                        program.Statements.Count.ToString()));

                var statement = program.Statements[0];

                if (statement.GetType() != typeof(ExpressionStatement))
                    throw new Exception(string.Format("Expected 'ExpressionStatement', but got {0}", statement));

                var expression = statement.Expression;

                if (expression.GetType() != typeof(PrefixExpression))
                    throw new Exception(string.Format("Expected 'PrefixExpression', but got {0}", expression));

                if (expression.Operator != prefixStruct.Operator)
                    throw new Exception(string.Format("Expected '{0}', but got '{1}'", prefixStruct.Operator,
                        expression.Operator));

                Assert.True(TestIntegerLiteral(expression.Right, prefixStruct.IntegerValue));
            }
        }

        [Fact]
        public void TestReturnStatements()
        {
            const string input = @"
                    return 5;
                    return 7;
                    return 2995;
                ";

            var lexer = new Lexer(input);

            var parser = new Parser(lexer);

            var program = parser.ParseProgram();
            checkParserErrors(parser);

            if (program == null) throw new Exception("Program is null.");

            if (program.Statements.Count != 3)
                throw new Exception(string.Format("{0} statements was founded, but expected 3",
                    program.Statements.Count.ToString()));

            foreach (var statement in program.Statements)
            {
                var returnStatement = statement.GetType();

                if (returnStatement != typeof(ReturnStatement))
                    throw new Exception(string.Format("Expected 'ReturnStatement', but got {0}", returnStatement));

                if (statement.TokenLiteral() != "return")
                    throw new Exception(string.Format("Expected 'return', but got {0}", statement.TokenLiteral()));
            }
        }
    }
}