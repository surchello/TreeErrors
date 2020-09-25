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

            var firstResult = inputKeeper.GetFirstChild('A');
            Assert.True(firstResult.IsSuccess);
            Assert.Equal('B', firstResult.Result);

            var secondResult = inputKeeper.GetFirstChild('C');
            Assert.True(secondResult.IsSuccess);
            Assert.Equal('D', secondResult.Result);
        }

        [Fact]
        public void FindMultipleChildren()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('A', 'D')
            };

            var inputKeeper = InputKeeper<char>.Create(items).Result!;

            var firstResult = inputKeeper.GetFirstChild('A');
            Assert.True(firstResult.IsSuccess);
            Assert.Equal('B', firstResult.Result);

            var secondResult = inputKeeper.GetSecondChild('A');
            Assert.True(secondResult.IsSuccess);
            Assert.Equal('D', secondResult.Result);
        }

        [Fact]
        public void FindOnlyFirstChild()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B')
            };

            var inputKeeper = InputKeeper<char>.Create(items).Result!;

            var firstResult = inputKeeper.GetFirstChild('A');
            Assert.True(firstResult.IsSuccess);
            Assert.Equal('B', firstResult.Result);

            var secondResult = inputKeeper.GetSecondChild('A');
            Assert.False(secondResult.IsSuccess);
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

            var parentResult = inputKeeper.GetPrimaryParent('B');
            Assert.True(parentResult.IsSuccess);
            Assert.Equal('A', parentResult.Result);
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

            var parentsResult = inputKeeper.GetAdditionalParents('B');
            Assert.True(parentsResult.IsSuccess);
            Assert.Equal(2, parentsResult.Result.Count());
            Assert.Contains('C', parentsResult.Result);
            Assert.Contains('D', parentsResult.Result);
        }

        [Fact]
        public void AdditionalParentsNotFound()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('C', 'D')
            };

            var inputKeeper = InputKeeper<char>.Create(items).Result!;

            var parentsResult = inputKeeper.GetAdditionalParents('B');
            Assert.False(parentsResult.IsSuccess);
        }
    }
}
