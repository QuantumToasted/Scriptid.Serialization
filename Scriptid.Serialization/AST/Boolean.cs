using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public class Boolean : Node
    {
        public Boolean(bool value)
        {
            Value = value;
        }

        [DataMember(Name = "value", IsRequired = true)]
        public bool Value { get; internal set; }
    }
}