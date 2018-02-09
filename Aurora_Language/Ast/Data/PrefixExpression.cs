using System;
using System.Collections.Generic;

namespace Aurora_Language.Data
{
    public class PrefixExpression : IExpression
    {
        public Token Token { get; set; }

        public PrefixExpression(Token token, string @operator)
        {
            Token = token;
            Operator = @operator;
        }

        public bool BoolValue { get; set; }

        string IExpression.Operator { get; set; }

        public IExpression Left { get; set; }
        public IExpression Value { get; set; }
        public Identifier Name { get; set; }
        public IExpression ReturnValue { get; set; }
        public IExpression Right { get; set; }

        public IExpression Condition { get; set; }
        public BlockStatement Consequence { get; set; }
        public BlockStatement Alternative { get; set; }
        public List<Identifier> Parameters { get; set; }
        public IExpression Function { get; set; }
        public List<IExpression> Arguments { get; set; }
        public BlockStatement Body { get; set; }

        public IExpression Expression { get; set; }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public string StringValue { get; set; }
        public string Operator { get; set; }

        public List<IStatement> Statements { get; set; }
        public long LongValue { get; set; }

        public void ExpressionNode()
        {
            throw new NotImplementedException();
        }
    }
}