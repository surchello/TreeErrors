using System.Collections.Generic;
using System.Linq;
using Xunit;
using ImplCore.Input;

namespace Tests
{
    public class InputKeeperPositiveTest
    {
        [Fact]
        public void SuccessCreate()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('C', 'D')
            };

            var inputKeeperResult = InputKeeper<char>.Create(items);
            Assert.True(inputKeeperResult.IsSuccess);

            var inputKeeper = inputKeeperResult.Result!;

            var isFirstFound = inputKeeper.TryGetFirstChild('A', out char firstChild);
            Assert.True(isFirstFound);
            Assert.Equal('B', firstChild);
        }

        [Fact]
        public void FindMultipleChildren()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('A', 'D')
            };

            var inputKeeper = InputKeeper<char>.Create(items).Result!;

            var isFirstFound = inputKeeper.TryGetFirstChild('A', out char firstChild);
            Assert.True(isFirstFound);
            Assert.Equal('B', firstChild);

            var isSecondFound = inputKeeper.TryGetSecondChild('A', out char secondChild);
            Assert.True(isSecondFound);
            Assert.Equal('D', secondChild);
        }

        [Fact]
        public void FindOnlyFirstChild()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B')
            };

            var inputKeeper = InputKeeper<char>.Create(items).Result!;

            var isFirstFound = inputKeeper.TryGetFirstChild('A', out char firstChild);
            Assert.True(isFirstFound);
            Assert.Equal('B', firstChild);

            var isSecondFound = inputKeeper.TryGetSecondChild('A', out char secondChild);
            Assert.False(isSecondFound);
        }

        [Fact]
        public void FindFirstItem()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B')
            };

            var inputKeeper = InputKeeper<char>.Create(items).Result!;

            var firstItem = inputKeeper.GetFirstItem();
            Assert.NotNull(firstItem);
        }

        [Fact]
        public void FindPrimaryParent()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('C', 'B'),
                new InputItem<char>('D', 'B')
            };

            var inputKeeper = InputKeeper<char>.Create(items).Result!;

            var isParentFound = inputKeeper.TryGetPrimaryParent('B', out char parent);
            Assert.True(isParentFound);
            Assert.Equal('A', parent);
        }

        [Fact]
        public void FindAdditionalParents()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('C', 'B'),
                new InputItem<char>('D', 'B')
            };

            var inputKeeper = InputKeeper<char>.Create(items).Result!;

            var isParentsFound = inputKeeper.TryGetAdditionalParents('B', out IEnumerable<char>? parents);
            Assert.True(isParentsFound);
            Assert.Equal(2, parents.Count());
            Assert.Contains('C', parents);
            Assert.Contains('D', parents);
        }

        [Fact]
        public void AdditionalParentsNotFound()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('C', 'D')
            };

            var inputKeeper = InputKeeper<char>.Create(items).Result!;

            var isParentsFound = inputKeeper.TryGetAdditionalParents('B', out IEnumerable<char>? parents);
            Assert.False(isParentsFound);
        }
    }
}
