using System.Collections.Generic;
using Aurora_Language.Data;

namespace Aurora_Language
{
    public class Program : INode
    {
        public Program()
        {
            Statements = new List<IStatement>();
        }

        public IExpression Right { get; set; }
        public IExpression Left { get; set; }
        public IExpression Value { get; set; }
        public Token Token { get; set; }
        public Identifier Name { get; set; }
        public IExpression ReturnValue { get; set; }
        public List<Identifier> Parameters { get; set; }
        public BlockStatement Body { get; set; }
        public IExpression Function { get; set; }
        public List<IExpression> Arguments { get; set; }
        public string StringValue { get; set; }
        public string Operator { get; set; }
        public List<IStatement> Statements { get; set; }

        public IExpression Expression { get; set; }

        public string TokenLiteral()
        {
            return Statements.Count > 0 ? Statements[0].TokenLiteral() : "";
        }
    }
}