using Xunit;
using ImplCore.Input;

namespace Tests
{
    public class InputParserTest
    {
        [Fact]
        public void SuccessValidInput()
        {
            var parser = GetInputParser();
            var result = parser.Parse("(A,B) (C,D)");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Result);
            Assert.Equal(2, result.Result!.Count);
            Assert.Contains(result.Result, i => i.Parent == 'A' && i.Child == 'B');
            Assert.Contains(result.Result, i => i.Parent == 'C' && i.Child == 'D');
        }

        private InputParser<char> GetInputParser() => new InputParser<char>(new CharInputItemParser());
    }
}
