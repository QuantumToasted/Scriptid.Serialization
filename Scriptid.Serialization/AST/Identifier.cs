using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public sealed class Identifier : Node
    {
        internal Identifier(string name)
        {
            Name = name;
        }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; internal set; }
    }
}