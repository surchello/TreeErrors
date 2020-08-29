using ImplCore.General;
using ImplCore.Tree;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ImplCore.Input
{
    internal class InputKeeper : IInputKeeper
    {
        private Dictionary<char, char> firstChildren;
        private Dictionary<char, char> secondChildren;
        private Dictionary<char, char> primaryParents;
        private Dictionary<char, IList<char>> additionalParents;

        private InputKeeper(Dictionary<char, char> firstChildren,
            Dictionary<char, char> secondChildren,
            Dictionary<char, char> primaryParents,
            Dictionary<char, IList<char>> additionalParents)
        {
            this.firstChildren = firstChildren;
            this.secondChildren = secondChildren;
            this.primaryParents = primaryParents;
            this.additionalParents = additionalParents;
    }

        public static OperationResult<InputKeeper> Create(IEnumerable<InputItem> inputItems)
        {
            var firstChildren = new Dictionary<char, char>();
            var secondChildren = new Dictionary<char, char>();

            var primaryParents = new Dictionary<char, char>();
            var additionalParents = new Dictionary<char, IList<char>>();

            foreach (var inputItem in inputItems)
            {
                ErrorCode? addResult = AddChild(firstChildren, secondChildren, inputItem);
                if (addResult.HasValue)
                {
                    return OperationResult<InputKeeper>.Error(addResult.Value);
                }

                AddParent(primaryParents, additionalParents, inputItem);
            }

            var inputKeeper = new InputKeeper(firstChildren, secondChildren, primaryParents, additionalParents);

            return OperationResult<InputKeeper>.Success(inputKeeper);
        }

        public InputItem? GetFirstItem()
        {
            if (!firstChildren.Any())
            {
                return null;
            }

            var firstInput = firstChildren.First();
            return new InputItem(firstInput.Key, firstInput.Value);
        }

        public bool TryGetFirstChild(char parent, out char child) => firstChildren.TryGetValue(parent, out child);

        public bool TryGetSecondChild(char parent, out char child) => secondChildren.TryGetValue(parent, out child);

        public bool TryGetPrimaryParent(char child, out char parent) => primaryParents.TryGetValue(child, out parent);
        
        public bool TryGetAdditionalParents(char child, [MaybeNullWhen(false)] out IEnumerable<char> parents)
        {
            if (additionalParents.TryGetValue(child, out IList<char>? p))
            {
                parents = p;
                return true;
            }

            parents = default;
            return false;
        }

        private static ErrorCode? AddChild(Dictionary<char, char> firstChildren,
            Dictionary<char, char> secondChildren,
            InputItem inputItem)
        {
            AddItemResult addResult = AddChild(firstChildren, inputItem);
            if (addResult.IsAdded)
            {
                return null;
            }

            if (addResult.ErrorCode.HasValue)
            {
                return addResult.ErrorCode.Value;
            }

            addResult = AddChild(secondChildren, inputItem);
            if (addResult.IsAdded)
            {
                return null;
            }

            if (addResult.ErrorCode.HasValue)
            {
                return addResult.ErrorCode.Value;
            }

            return ErrorCode.TooManyChildren;
        }

        private static AddItemResult AddChild(Dictionary<char, char> children, InputItem inputItem)
        {
            if (!children.TryGetValue(inputItem.Parent, out char existingChild))
            {
                children.Add(inputItem.Parent, inputItem.Child);
                return AddItemResult.Added();
            }

            if (existingChild == inputItem.Child)
            {
                return AddItemResult.Error(ErrorCode.DuplicatePair);
            }

            return AddItemResult.NotAdded();
        }

        private static void AddParent(Dictionary<char, char> primaryParents,
            Dictionary<char, IList<char>> additionalParents,
            InputItem inputItem)
        {
            if (!primaryParents.TryAdd(inputItem.Child, inputItem.Parent))
            {
                if (additionalParents.TryGetValue(inputItem.Child, out IList<char>? parents))
                {
                    parents.Add(inputItem.Parent);
                }
                else
                {
                    parents = new List<char> { inputItem.Parent };
                    additionalParents.Add(inputItem.Child, parents);
                }
            }
        }
    }
}
