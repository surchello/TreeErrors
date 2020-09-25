using System;
using System.Collections.Generic;
using System.Linq;
using ImplCore.General;
using ImplCore.Tree;

namespace ImplCore.Input
{
    internal class InputKeeper<T> : IInputKeeper<T> where T : notnull, IEquatable<T>
    {
        private Dictionary<T, T> firstChildren;
        private Dictionary<T, T> secondChildren;
        private Dictionary<T, T> primaryParents;
        private Dictionary<T, IList<T>> additionalParents;

        private InputKeeper(Dictionary<T, T> firstChildren,
            Dictionary<T, T> secondChildren,
            Dictionary<T, T> primaryParents,
            Dictionary<T, IList<T>> additionalParents)
        {
            this.firstChildren = firstChildren;
            this.secondChildren = secondChildren;
            this.primaryParents = primaryParents;
            this.additionalParents = additionalParents;
    }

        public static OperationResult<InputKeeper<T>> Create(IEnumerable<InputItem<T>> inputItems)
        {
            var firstChildren = new Dictionary<T, T>();
            var secondChildren = new Dictionary<T, T>();

            var primaryParents = new Dictionary<T, T>();
            var additionalParents = new Dictionary<T, IList<T>>();

            foreach (var inputItem in inputItems)
            {
                ErrorCode? addResult = AddChild(firstChildren, secondChildren, inputItem);
                if (addResult.HasValue)
                {
                    return OperationResult<InputKeeper<T>>.Error(addResult.Value);
                }

                AddParent(primaryParents, additionalParents, inputItem);
            }

            var inputKeeper = new InputKeeper<T>(firstChildren, secondChildren, primaryParents, additionalParents);

            return OperationResult<InputKeeper<T>>.Success(inputKeeper);
        }

        public InputItem<T>? GetFirstItem()
        {
            if (!firstChildren.Any())
            {
                return null;
            }

            var firstInput = firstChildren.First();
            return new InputItem<T>(firstInput.Key, firstInput.Value);
        }

        public SearchItemResult<T> GetFirstChild(T parent)
        {
            if (firstChildren.TryGetValue(parent, out T child))
            {
                return SearchItemResult<T>.Success(child);
            }

            return SearchItemResult<T>.Failed();
        }

        public SearchItemResult<T> GetSecondChild(T parent)
        {
            if (secondChildren.TryGetValue(parent, out T child))
            {
                return SearchItemResult<T>.Success(child);
            }

            return SearchItemResult<T>.Failed();
        }

        public SearchItemResult<T> GetPrimaryParent(T child)
        {
            if (primaryParents.TryGetValue(child, out T parent))
            {
                return SearchItemResult<T>.Success(parent);
            }

            return SearchItemResult<T>.Failed();
        }
        
        public SearchItemResult<IEnumerable<T>> GetAdditionalParents(T child)
        {
            if (additionalParents.TryGetValue(child, out IList<T>? parents))
            {
                return SearchItemResult<IEnumerable<T>>.Success(parents);
            }

            return SearchItemResult<IEnumerable<T>>.Failed();
        }

        private static ErrorCode? AddChild(Dictionary<T, T> firstChildren,
            Dictionary<T, T> secondChildren,
            InputItem<T> inputItem)
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

        private static AddItemResult AddChild(Dictionary<T, T> children, InputItem<T> inputItem)
        {
            if (!children.TryGetValue(inputItem.Parent, out T existingChild))
            {
                children.Add(inputItem.Parent, inputItem.Child);
                return AddItemResult.Added();
            }

            if (existingChild.Equals(inputItem.Child))
            {
                return AddItemResult.Error(ErrorCode.DuplicatePair);
            }

            return AddItemResult.NotAdded();
        }

        private static void AddParent(Dictionary<T, T> primaryParents,
            Dictionary<T, IList<T>> additionalParents,
            InputItem<T> inputItem)
        {
            if (!primaryParents.TryAdd(inputItem.Child, inputItem.Parent))
            {
                if (additionalParents.TryGetValue(inputItem.Child, out IList<T>? parents))
                {
                    parents.Add(inputItem.Parent);
                }
                else
                {
                    parents = new List<T> { inputItem.Parent };
                    additionalParents.Add(inputItem.Child, parents);
                }
            }
        }
    }
}
