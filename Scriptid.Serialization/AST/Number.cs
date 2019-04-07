using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public sealed class Number : Node
    {
        internal Number(int value)
        {
            Value = value;
        }

        [DataMember(Name = "value", IsRequired = true)]
        public int Value { get; internal set; }
    }
}