using Xunit;
using ImplCore.Input;
using ImplCore.Tree;
using System.Collections.Generic;
using ImplCore.General;

namespace Tests
{
    public class TreeBuilderTest
    {
        [Fact]
        public void SuccessEmptyTree()
        {
            var items = new InputItem<char>[0];
            var treeResult = GetTreeResult(items);

            Assert.True(treeResult.IsSuccess);
            Assert.Null(treeResult.Result);
        }

        [Fact]
        public void CorrectChildrenOrder()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'C'),
                new InputItem<char>('A', 'B')
            };
            var head = GetTreeResult(items).Result!;

            Assert.Equal('B', head.Left!.Value);
            Assert.Equal('C', head.Right!.Value);
        }

        [Fact]
        public void SuccessFullTree()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('A', 'C'),
                new InputItem<char>('B', 'D'),
                new InputItem<char>('B', 'E'),
                new InputItem<char>('C', 'F'),
                new InputItem<char>('C', 'G')
            };
            var treeResult = GetTreeResult(items);

            Assert.True(treeResult.IsSuccess);
            Assert.NotNull(treeResult.Result);

            var head = treeResult.Result!;

            Assert.Equal('A', head.Value);

            Assert.NotNull(head.Left);
            Assert.Equal('B', head.Left!.Value);
            Assert.NotNull(head.Left!.Left);
            Assert.NotNull(head.Left!.Right);
            Assert.Equal('D', head.Left!.Left!.Value);
            Assert.Equal('E', head.Left!.Right!.Value);

            Assert.NotNull(head.Right);
            Assert.Equal('C', head.Right!.Value);
            Assert.NotNull(head.Right!.Left);
            Assert.NotNull(head.Right!.Right);
            Assert.Equal('F', head.Right!.Left!.Value);
            Assert.Equal('G', head.Right!.Right!.Value);
        }

        [Fact]
        public void SuccessStartFromBottom()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('D', 'E'),
                new InputItem<char>('A', 'B'),
                new InputItem<char>('A', 'C'),
                new InputItem<char>('B', 'D')
            };
            var treeResult = GetTreeResult(items);

            Assert.True(treeResult.IsSuccess);
            Assert.NotNull(treeResult.Result);

            var head = treeResult.Result!;

            Assert.Equal('A', head.Value);

            Assert.NotNull(head.Left);
            Assert.Equal('B', head.Left!.Value);
            Assert.NotNull(head.Left!.Left);
            Assert.Null(head.Left!.Right);
            Assert.Equal('D', head.Left!.Left!.Value);
            Assert.NotNull(head.Left!.Left!.Left);
            Assert.Equal('E', head.Left!.Left!.Left!.Value);
        }

        [Fact]
        public void SelfNodeCycle()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('A', 'A')
            };
            var treeResult = GetTreeResult(items);

            Assert.False(treeResult.IsSuccess);
            Assert.Equal(ErrorCode.Cycle, treeResult.ErrorCode);
        }

        [Fact]
        public void Cycle()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('A', 'C'),
                new InputItem<char>('B', 'D'),
                new InputItem<char>('B', 'E'),
                new InputItem<char>('E', 'C')
            };
            var treeResult = GetTreeResult(items);

            Assert.False(treeResult.IsSuccess);
            Assert.Equal(ErrorCode.Cycle, treeResult.ErrorCode);
        }

        [Fact]
        public void MultipleRoots()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('A', 'C'),
                new InputItem<char>('K', 'B')
            };
            var treeResult = GetTreeResult(items);

            Assert.False(treeResult.IsSuccess);
            Assert.Equal(ErrorCode.MultipleRoots, treeResult.ErrorCode);
        }

        [Fact]
        public void MultipleRootsStartFromMiddle()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('A', 'C'),
                new InputItem<char>('D', 'A'),
                new InputItem<char>('D', 'E'),
                new InputItem<char>('K', 'E')
            };
            var treeResult = GetTreeResult(items);

            Assert.False(treeResult.IsSuccess);
            Assert.Equal(ErrorCode.MultipleRoots, treeResult.ErrorCode);
        }

        [Fact]
        public void CycleBeforeMultipleRoots()
        {
            var items = new InputItem<char>[] {
                new InputItem<char>('A', 'B'),
                new InputItem<char>('A', 'C'),
                new InputItem<char>('D', 'C'),
                new InputItem<char>('C', 'B')
            };
            var treeResult = GetTreeResult(items);

            Assert.False(treeResult.IsSuccess);
            Assert.Equal(ErrorCode.Cycle, treeResult.ErrorCode);
        }

        private OperationResult<Node<char>?> GetTreeResult(IEnumerable<InputItem<char>> items)
        {
            var inputKeeper = InputKeeper<char>.Create(items).Result!;
            return new TreeBuilder<char>().Build(inputKeeper);
        }
    }
}
