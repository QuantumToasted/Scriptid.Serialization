using System;
using Xunit;

namespace Scriptid.Tests
{
    public class UnitTest1
    {
        private const string CODE = @"
str test = ""Hello, World!"";
";
        [Fact]
        public void Test1()
        {
            var code = Serialization.Scriptid.Parse(CODE);
            Assert.NotEmpty(code.Steps);
            foreach (var step in code.Steps)
            {
                Assert.NotNull(step);
            }
        }
    }
}
