using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public sealed class Keyword : Node
    {
        internal Keyword(string word)
        {
            Word = word;
        }

        [DataMember(Name = "word", IsRequired = true)]
        public string Word { get; internal set; }
    }
}