using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ImplCore.Input
{
    internal class InputKeeper : IInputKeeper
    {
        private Dictionary<char, char> firstChilds;
        private Dictionary<char, char> secondChilds;
        private Dictionary<char, char> primaryParents;
        private Dictionary<char, IList<char>> additionalParents;

        private InputKeeper(Dictionary<char, char> firstChilds,
            Dictionary<char, char> secondChilds,
            Dictionary<char, char> primaryParents,
            Dictionary<char, IList<char>> additionalParents)
        {
            this.firstChilds = firstChilds;
            this.secondChilds = secondChilds;
            this.primaryParents = primaryParents;
            this.additionalParents = additionalParents;
    }

        public static InputKeeper Create(IEnumerable<InputItem> inputItems)
        {
            //TODO: validate params not null(add Guard)
            var firstChilds = new Dictionary<char, char>();
            var secondChilds = new Dictionary<char, char>();

            var primaryParents = new Dictionary<char, char>();
            var additionalParents = new Dictionary<char, IList<char>>();

            foreach (var inputItem in inputItems)
            {
                if (TryAddChild(firstChilds, inputItem))
                {
                    AddParent(primaryParents, additionalParents, inputItem);
                    continue;
                }

                if (TryAddChild(secondChilds, inputItem))
                {
                    AddParent(primaryParents, additionalParents, inputItem);
                    continue;
                }

                throw new Exception("more than two childs");
            }

            return new InputKeeper(firstChilds, secondChilds, primaryParents, additionalParents);
        }

        public InputItem? GetFirstItem()
        {
            if (!firstChilds.Any())
            {
                return null;
            }

            var firstInput = firstChilds.First();
            return new InputItem(firstInput.Key, firstInput.Value);
        }

        public bool TryGetFirstChild(char parent, out char child) => firstChilds.TryGetValue(parent, out child);

        public bool TryGetSecondChild(char parent, out char child) => secondChilds.TryGetValue(parent, out child);

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

        private static bool TryAddChild(Dictionary<char, char> childs, InputItem inputItem)
        {
            if (!childs.TryGetValue(inputItem.Parent, out char firstChild))
            {
                childs.Add(inputItem.Parent, inputItem.Child);
                return true;
            }

            if (firstChild == inputItem.Child)
            {
                throw new Exception("duplicate pair");
            }

            return false;//TODO: return false or error
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
