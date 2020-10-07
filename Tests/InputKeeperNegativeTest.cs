using Xunit;
using ImplCore.Input;
using ImplCore.General;

namespace Tests
{
    public class InputKeeperNegativeTest
    {
        [Fact]
        public void Duplicates()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('C', 'D'),
                new InputItem<char>('A', 'B')
            };

            var inputKeeperResult = InputKeeper<char>.Create(items);
            Assert.False(inputKeeperResult.IsSuccess);
            Assert.Equal(ErrorCode.DuplicatePair, inputKeeperResult.ErrorCode);
        }

        [Fact]
        public void TooManyChildren()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('A', 'D'),
                new InputItem<char>('A', 'C')
            };

            var inputKeeperResult = InputKeeper<char>.Create(items);
            Assert.False(inputKeeperResult.IsSuccess);
            Assert.Equal(ErrorCode.TooManyChildren, inputKeeperResult.ErrorCode);
        }
    }
}
