using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public class Identifier : Node
    {
        public Identifier(string name)
        {
            Name = name;
        }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; internal set; }
    }
}