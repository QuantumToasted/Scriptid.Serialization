using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public class If : Node
    {
        public If(Node condition, Node then, Node @else)
        {
            Condition = condition;
            Then = then;
            Else = @else;
        }

        [DataMember(Name = "condition", IsRequired = true)]
        public Node Condition { get; internal set; }

        [DataMember(Name = "then", IsRequired = true)]
        public Node Then { get; internal set; }

        [DataMember(Name = "else", IsRequired = false)]
        public Node Else { get; internal set; }
    }
}