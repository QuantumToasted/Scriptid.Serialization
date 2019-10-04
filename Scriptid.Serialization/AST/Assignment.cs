using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public class Assignment : Node
    {
        public Assignment(Node left, Node right)
        {
            Left = left;
            Right = right;
        }
        
        [DataMember(Name = "left", IsRequired = true)]
        public Node Left { get; internal set; }

        [DataMember(Name = "Right", IsRequired = true)]
        public Node Right { get; internal set; }
    }
}