using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public sealed class Boolean : Node
    {
        internal Boolean(bool value)
        {
            Value = value;
        }

        [DataMember(Name = "value", IsRequired = true)]
        public bool Value { get; internal set; }
    }
}