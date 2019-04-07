using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public sealed class String : Node
    {
        internal String(string value)
        {
            Value = value;
        }

        [DataMember(Name = "value", IsRequired = true)]
        public string Value { get; internal set; }
    }
}