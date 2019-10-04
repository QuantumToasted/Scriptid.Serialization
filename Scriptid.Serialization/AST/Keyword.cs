using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public class Keyword : Node
    {
        public Keyword(string word)
        {
            Word = word;
        }

        [DataMember(Name = "word", IsRequired = true)]
        public string Word { get; internal set; }
    }
}