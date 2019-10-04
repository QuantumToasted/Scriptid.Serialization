using System;
using System.Runtime.Serialization;

namespace Scriptid.Serialization.AST
{
    public class Program : Node
    {
        public Program(Node[] steps)
        {
            Steps = steps;
        }

        [DataMember(Name = "steps", IsRequired = true)]
        public Node[] Steps { get; internal set; }

        public static Program Empty => new Program(Array.Empty<Node>());
    }
}