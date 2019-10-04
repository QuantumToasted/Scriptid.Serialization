using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public class Number : Node
    {
        public Number(int value)
        {
            Value = value;
        }

        [DataMember(Name = "value", IsRequired = true)]
        public int Value { get; internal set; }
    }
}