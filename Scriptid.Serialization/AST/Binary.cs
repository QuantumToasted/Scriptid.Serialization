using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public sealed class Binary : Node
    {
        internal Binary(Operator @operator, Node left, Node right)
        {
            Operator = @operator;
            Left = left;
            Right = right;
        }

        [DataMember(Name = "operator", IsRequired = true)]
        public Operator Operator { get; internal set; }

        [DataMember(Name = "left", IsRequired = true)]
        public Node Left { get; internal set; }

        [DataMember(Name = "right", IsRequired = true)]
        public Node Right { get; internal set; }
    }
}