using System;
using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public sealed class Call : Node
    {
        internal Call(Func<Node> func, Node[] args)
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