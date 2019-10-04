using System;
using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public class Call : Node
    {
        public Call(Func<Node> func, Node[] args)
        {
            Func = func;
            Args = args;
        }

        [DataMember(Name = "func", IsRequired = true)]
        public Func<Node> Func { get; internal set; }

        [DataMember(Name = "args", IsRequired = true)]
        public Node[] Args { get; internal set; }
    }
}