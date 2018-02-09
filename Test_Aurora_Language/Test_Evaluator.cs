using System;
using Aurora_Language;
using Aurora_Language.Obj;
using Xunit;
using Boolean = Aurora_Language.Obj.Boolean;
using Environment = Aurora_Language.Obj.Environment;

namespace Test_Aurora_Language
{
    public class Test_Evaluator
    {
        private struct EvalIntegerStruct
        {
            public readonly string Input;
            public readonly long? Expected;

            public EvalIntegerStruct(string input, long? expected)
            {
                Input = input;
                Expected = expected;
            }
        }

        private struct ErrorStruct
        {
            public readonly string Input;
            public readonly string Expected;

            public ErrorStruct(string input, string expected)
            {
                Input = input;
                Expected = expected;
            }
        }

        private struct EvalBoolStruct
        {
            public readonly string Input;
            public readonly bool Expected;

            public EvalBoolStruct(string input, bool expected)
            {
                Input = input;
                Expected = expected;
            }
        }

        private static IObject TestEval(string input)
        {
            var lexer = new Lexer(input);

            var parser = new Parser(lexer);

            var program = parser.ParseProgram();

            var enviroment = new Environment();

            return Evaluator.Evaluate(program, enviroment);
        }

        private static bool TestIntegerObjects(IObject obj, long? expected)
        {
            if (obj.GetType() != typeof(Integer))
                throw new Exception(string.Format("Expected 'Integer', but got {0}", obj.GetType()));

            if (obj.LongValue != expected)
                throw new Exception(string.Format("Expected '{1}', but got {0}", obj.LongValue, expected));

            return true;
        }

        private static bool TestNullObjects(IObject obj)
        {
            return obj == null;
        }

        private static bool TestBooleanObjects(IObject obj, bool expected)
        {
            if (obj.GetType() != typeof(Boolean))
                throw new Exception(string.Format("Expected 'Boolean', but got {0}", obj.GetType()));

            if (obj.BoolValue != expected)
                throw new Exception(string.Format("Expected '{1}', but got {0}", obj.BoolValue, expected));

            return true;
        }

        [Fact]
        public void TestBangExpression()
        {
            var data = new EvalBoolStruct[6];

            data[0] = new EvalBoolStruct("!true;", false);
            data[1] = new EvalBoolStruct("!false;", true);
            data[2] = new EvalBoolStruct("!7;", false);
            data[3] = new EvalBoolStruct("!!7;", true);
            data[4] = new EvalBoolStruct("!!false;", false);
            data[5] = new EvalBoolStruct("!!true;", true);

            for (var i = 0; i < data.Length; i++)
            {
                var evaluated = TestEval(data[i].Input);
                Assert.True(TestBooleanObjects(evaluated, data[i].Expected));
            }
        }

        [Fact]
        public void TestBooleanExpression()
        {
            var data = new EvalBoolStruct[8];

            data[0] = new EvalBoolStruct("true;", true);
            data[1] = new EvalBoolStruct("false;", false);
            data[2] = new EvalBoolStruct("1 > 2;", false);
            data[3] = new EvalBoolStruct("1 < 2;", true);
            data[4] = new EvalBoolStruct("1 == 1;", true);
            data[5] = new EvalBoolStruct("1 != 2;", true);
            data[6] = new EvalBoolStruct("true == true;", true);
            data[7] = new EvalBoolStruct("true == false;", false);

            for (var i = 0; i < data.Length; i++)
            {
                var evaluated = TestEval(data[i].Input);
                Assert.True(TestBooleanObjects(evaluated, data[i].Expected));
            }
        }

        [Fact]
        public void TestErrorHandling()
        {
            var data = new ErrorStruct[8];

            data[0] = new ErrorStruct("5 + true;", "type mismatch: INTEGER+BOOLEAN");
            data[1] = new ErrorStruct("5 + true; 5;", "type mismatch: INTEGER+BOOLEAN");
            data[2] = new ErrorStruct("-true;", "Unknown operator: -BOOLEAN");
            data[3] = new ErrorStruct("true + false;", "Unknown operator: BOOLEAN+BOOLEAN");
            data[4] = new ErrorStruct("77; true + false; 66;", "Unknown operator: BOOLEAN+BOOLEAN");
            data[5] = new ErrorStruct("if(10 > 2) {true + true}", "Unknown operator: BOOLEAN+BOOLEAN");
            data[6] = new ErrorStruct(@"
                    if (25 > 5) {
                        if (10 > 2) {
                            return true + false;
                        }
                        return 1;
                    }
                ", "Unknown operator: BOOLEAN+BOOLEAN");
            data[7] = new ErrorStruct("foobar", "Identifier not found: foobar");

            for (var i = 0; i < data.Length; i++)
            {
                var evaluated = TestEval(data[i].Input);

                if (evaluated.GetType() == typeof(Error)) Assert.True(evaluated.Message == data[i].Expected);
            }
        }

        [Fact]
        public void TestFunctionApp()
        {
            var data = new EvalIntegerStruct[2];

            data[0] = new EvalIntegerStruct("let identity = fn(x) { x; }; identity(5);", 5);
            data[1] = new EvalIntegerStruct("let add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20);

            for (var i = 0; i < data.Length; i++)
            {
                var evaluated = TestEval(data[i].Input);
                Assert.True(TestIntegerObjects(evaluated, data[i].Expected));
            }
        }

        [Fact]
        public void TestFunctionObject()
        {
            var input = "fn(x) { x + 2; };";

            var evaluated = TestEval(input);

            if (evaluated.GetType() != typeof(Function))
                throw new Exception(string.Format("Expected 'Function', but got {0}", evaluated.GetType()));

            if (evaluated.Parameters.Count != 1)
                throw new Exception(string.Format("Expected '1', but got {0}", evaluated.Parameters.Count));

            if (evaluated.Parameters[0].TokenLiteral() != "x")
                throw new Exception(string.Format("Expected 'x', but got {0}", evaluated.Parameters[0].TokenLiteral()));

            var expectedBody = "(x + 2)";

            if (evaluated.Body.TokenLiteral() != expectedBody)
                throw new Exception(string.Format("Expected '{1}', but got {0}", evaluated.Body.TokenLiteral(),
                    expectedBody));
        }

        [Fact]
        public void TestIfElseExpression()
        {
            var data = new EvalIntegerStruct[6];

            data[0] = new EvalIntegerStruct("if (true) {10};", 10);
            data[1] = new EvalIntegerStruct("if (false) {5};", null);
            data[2] = new EvalIntegerStruct("if (1 > 2) {10} else {5};", 5);
            data[3] = new EvalIntegerStruct("if (1 < 2) {10};", 10);
            data[4] = new EvalIntegerStruct("if (1) {1};", 1);
            data[5] = new EvalIntegerStruct("if(false) {10};", null);

            for (var i = 0; i < data.Length; i++)
            {
                var evaluated = TestEval(data[i].Input);
                Assert.True(data[i].Expected == null
                    ? TestNullObjects(evaluated)
                    : TestIntegerObjects(evaluated, data[i].Expected));
            }
        }

        [Fact]
        public void TestIntegerExpression()
        {
            var data = new EvalIntegerStruct[12];

            data[0] = new EvalIntegerStruct("5;", 5);
            data[1] = new EvalIntegerStruct("7;", 7);
            data[2] = new EvalIntegerStruct("-5;", -5);
            data[3] = new EvalIntegerStruct("-7;", -7);
            data[4] = new EvalIntegerStruct("1 + 1;", 2);
            data[5] = new EvalIntegerStruct("23 + 7;", 30);
            data[6] = new EvalIntegerStruct("1 - 1;", 0);
            data[7] = new EvalIntegerStruct("25 - 5;", 20);
            data[8] = new EvalIntegerStruct("1 * 1;", 1);
            data[9] = new EvalIntegerStruct("6 * 6;", 36);
            data[10] = new EvalIntegerStruct("49 / 7;", 7);
            data[11] = new EvalIntegerStruct("100 / 10;", 10);

            for (var i = 0; i < data.Length; i++)
            {
                var evaluated = TestEval(data[i].Input);
                Assert.True(TestIntegerObjects(evaluated, data[i].Expected));
            }
        }

        [Fact]
        public void TestLetStatements()
        {
            var data = new EvalIntegerStruct[2];

            data[0] = new EvalIntegerStruct("let a = 5;", 5);
            data[1] = new EvalIntegerStruct("let a = 2; let b = 5; let c = a + b;", 7);

            for (var i = 0; i < data.Length; i++)
            {
                var evaluated = TestEval(data[i].Input);
                Assert.True(TestIntegerObjects(evaluated, data[i].Expected));
            }
        }

        [Fact]
        public void TestReturnStatements()
        {
            var data = new EvalIntegerStruct[5];

            data[0] = new EvalIntegerStruct("return 10;", 10);
            data[1] = new EvalIntegerStruct("return 10; 9;", 10);
            data[2] = new EvalIntegerStruct("return 5 * 5; 77;", 25);
            data[3] = new EvalIntegerStruct("77; return 2 * 2; 88;", 4); // TODO: Fix this.

            data[4] = new EvalIntegerStruct(@"
                    if (25 > 5) {
                        if (10 > 2) {
                            return 77;
                        }
                    }
                ", 77);

            for (var i = 0; i < data.Length; i++)
            {
                var evaluated = TestEval(data[i].Input);
                Assert.True(TestIntegerObjects(evaluated, data[i].Expected));
            }
        }
    }
}