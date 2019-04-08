using System;
using Xunit;

namespace Scriptid.Tests
{
    public class UnitTest1
    {
        private const string CODE = @"
str test = ""Hello, World!"";
num test2 = 5 + 5;
test2 = test2 + 25;
num test3 = test2 + 10;
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
