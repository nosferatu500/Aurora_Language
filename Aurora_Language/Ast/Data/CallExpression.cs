﻿using System;
using System.Collections.Generic;

namespace Aurora_Language.Data
{
    public class CallExpression : IExpression
    {
        public Token Token{ get; set; }

        public CallExpression(Token token, IExpression function)
        {
            Token = token;
            Function = function;
        }

        public IExpression Function { get; set; }
        public List<IExpression> Arguments { get; set; }

        public IExpression Expression { get; set; }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public Identifier Name { get; set; }
        public IExpression ReturnValue { get; set; }
        public string StringValue { get; set; }
        public List<IStatement> Statements { get; set; }
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
        public BlockStatement Body { get; set; }

        public void ExpressionNode()
        {
            throw new NotImplementedException();
        }
    }
}