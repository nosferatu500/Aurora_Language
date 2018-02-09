using Aurora_Language.Data;

namespace Aurora_Language
{
    public interface IStatement : INode
    {
        new Identifier Name { get; set; }


        void StatementNode();
    }
}