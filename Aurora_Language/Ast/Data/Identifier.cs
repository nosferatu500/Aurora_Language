using System.Collections.Generic;

namespace Aurora_Language.Data
{
    public class Identifier : IExpression
    {
        public Token Token{ get; set; }

        public Identifier(Token token, string value)
        {
            Token = token;
            StringValue = value;
        }

        public Identifier Name { get; set; }
        public IExpression ReturnValue { get; set; }
        public string StringValue { get; set; }
        public List<IStatement> Statements { get; set; }
        public IExpression Expression { get; set; }
        public long LongValue { get; set; }
        public bool BoolValue { get; set; }
        public string Operator { get; set; }
        public IExpression Left { get; set; }
        public IExpression Value { get; set; }
        public IExpression Right { get; set; }
        public IExpression Condition { get; set; }
        public BlockStatement Consequence { get; set; }
        public BlockStatement Alternative { get; set; }
        public List<Identifier> Parameters { get; set; }
        public IExpression Function { get; set; }
        public List<IExpression> Arguments { get; set; }
        public BlockStatement Body { get; set; }

        void IExpression.ExpressionNode()
        {
            ExpressionNode();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        private static void ExpressionNode()
        {
        }
    }
}