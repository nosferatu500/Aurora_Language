using System.Collections.Generic;
using Aurora_Language.Data;

namespace Aurora_Language
{
    public interface IExpression : INode
    {
        long LongValue { get; set; }
        bool BoolValue { get; set; }

        new string Operator { get; set; }

        new IExpression Left { get; set; }
        new IExpression Right { get; set; }

        BlockStatement Consequence { get; set; }
        BlockStatement Alternative { get; set; }

        new List<Identifier> Parameters { get; set; }

        new List<IExpression> Arguments { get; set; }

        new BlockStatement Body { get; }

        void ExpressionNode();
    }
}