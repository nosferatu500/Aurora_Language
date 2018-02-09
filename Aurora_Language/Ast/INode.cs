using System.Collections.Generic;
using Aurora_Language.Data;

namespace Aurora_Language
{
    public interface INode
    {
        IExpression Right { get; set; }
        IExpression Left { get; set; }
        IExpression Value { get; set; }
        
        Token Token { get; set; }

        Identifier Name { get; set; }

        IExpression ReturnValue { get; set; }

        List<Identifier> Parameters { get; set; }
        BlockStatement Body { get; set; }

        IExpression Function { get; set; }
        List<IExpression> Arguments { get; set; }

        string StringValue { get; set; }
        string Operator { get; set; }
        List<IStatement> Statements { get; set; }
        IExpression Expression { get; set; }

        string TokenLiteral();
    }
}