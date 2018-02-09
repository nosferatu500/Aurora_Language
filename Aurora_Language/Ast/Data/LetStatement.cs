using System.Collections.Generic;

namespace Aurora_Language.Data
{
    public class LetStatement : IStatement
    {
        public Token Token{ get; set; }

        public LetStatement(Token token)
        {
            Token = token;
        }

        public IExpression Value { get; set; }

        public Identifier Name { get; set; }
        public IExpression Right { get; set; }
        public IExpression Left { get; set; }
        public IExpression ReturnValue { get; set; }
        public List<Identifier> Parameters { get; set; }
        public BlockStatement Body { get; set; }
        public IExpression Function { get; set; }
        public List<IExpression> Arguments { get; set; }
        public string StringValue { get; set; }
        public string Operator { get; set; }
        public List<IStatement> Statements { get; set; }
        public IExpression Expression { get; set; }

        void IStatement.StatementNode()
        {
            StatementNode();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        private static void StatementNode()
        {
        }
    }
}