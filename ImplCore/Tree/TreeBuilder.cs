using ImplCore.General;
using ImplCore.Input;
using System;
using System.Collections.Generic;

namespace ImplCore.Tree
{
    public class TreeBuilder//TODO: char generic?
    {
        private HashSet<char>? processedValues;
        private Node<char>? firstRoot;
        private int rootsCount;

        public TreeResult<char> Build(IInputKeeper inputKeeper)
        {
            InitNewBuild();
            try
            {
                var head = GetStartHead(inputKeeper);
                if (head == null)
                {
                    return TreeResult<char>.Success(head);
                }

                BuildDown(head, inputKeeper);
                BuildUp(head, inputKeeper);
            }
            catch (TreeBuildException ex)
            {
                return TreeResult<char>.Error(ex.ErrorCode);
            }

            if (rootsCount > 1)
            {
                return TreeResult<char>.Error(ErrorCode.MultipleRoots);
            }

            return TreeResult<char>.Success(firstRoot);
        }

        private Node<char>? GetStartHead(IInputKeeper inputKeeper)
        {
            Input.InputItem? firstItem = inputKeeper.GetFirstItem();
            if (!firstItem.HasValue)
            {
                return null;
            }

            var head = new Node<char>(firstItem.Value.Parent);
            if (!TrySaveToProcessed(head.Value))
            {
                throw new Exception($"First item is already processed.");
            }

            return head;
        }

        private void BuildUp(Node<char> head, IInputKeeper inputKeeper)
        {
            if (!inputKeeper.TryGetPrimaryParent(head.Value, out char parent))
            {
                //this method could return a collection of roots but we don't need  to know all of them
                //what we need to know is whether there is only one head. If yes we need a reference to that head.
                firstRoot ??= head;
                rootsCount++;

                return;
            }

            var newHead = ProcessParent(parent, head, inputKeeper);

            if (inputKeeper.TryGetAdditionalParents(head.Value, out IEnumerable<char> additionalParents))
            {
                //it's multiple heads but loop is still possible. We have to continue to determine what it is.
                foreach (var additionalParent in additionalParents)
                {
                    ProcessParent(additionalParent, newHead, inputKeeper);
                }
            }

            Node<char> ProcessParent(char parentValue, Node<char> oldHead, IInputKeeper inputKeeper)
            {
                var newHead = AttachNewHeadDeep(parentValue, oldHead, inputKeeper);
                BuildUp(newHead, inputKeeper);

                return newHead;
            }

            #region old parents
            //var parents = inputKeeper.GetByChild(head.Value);
            //if (!parents.Any())
            //{
            //    //TODO: Maybe just push it to global "heads collection"?
            //    //Currently parentRoots are uselessly copied on every run of BuildUp.
            //    return new List<Node<char>> { head };
            //}

            //var roots = new List<Node<char>>(parents.Count);

            //foreach (var parent in parents)//it's either multiple heads or loop. Continue to determine what it is.
            //{
            //    head = AttachNewHeadDeep(parent, head, inputKeeper);
            //    var parentRoots = BuildUp(head, inputKeeper);

            //    roots.AddRange(parentRoots);
            //}
            #endregion
        }

        private Node<char> AttachNewHeadDeep(char newHeadValue, Node<char> oldHead, IInputKeeper inputKeeper)
        {
            bool isFirstFound = inputKeeper.TryGetFirstChild(newHeadValue, out char firstChild);
            if (!isFirstFound)
            {
                throw new Exception($"Children not found for value {newHeadValue}. At least one is expected: {oldHead.Value}");
            }

            Node<char> newHead;

            bool isSecondFound = inputKeeper.TryGetSecondChild(newHeadValue, out char secondChild);
            if (!isSecondFound)
            {
                newHead = AttachParent(newHeadValue, oldHead, isLeft: true);
                return newHead;
            }

            char otherChildValue = oldHead.Value == firstChild ? firstChild : secondChild;
            bool isOldHeadLeft = IsFirstLeft(oldHead.Value, otherChildValue);

            newHead = AttachParent(newHeadValue, oldHead, isOldHeadLeft);
            var otherChildNode = AttachChild(newHead, otherChildValue, !isOldHeadLeft);

            BuildDown(otherChildNode, inputKeeper);

            return newHead;
        }

        private void BuildDown(Node<char> head, IInputKeeper inputKeeper)
        {
            bool isFirstFound = inputKeeper.TryGetFirstChild(head.Value, out char firstChild);
            bool isSecondFound = inputKeeper.TryGetSecondChild(head.Value, out char secondChild);
            if (!isFirstFound && !isSecondFound)
            {
                return;
            }

            if (isFirstFound)
            {
                bool isLeft = !isSecondFound || IsFirstLeft(firstChild, secondChild);
                var childNode = AttachChild(head, firstChild, isLeft);
                BuildDown(childNode, inputKeeper);
            }

            if (isSecondFound)
            {
                bool isLeft = !isFirstFound || IsFirstLeft(secondChild, firstChild);
                var childNode = AttachChild(head, secondChild, isLeft);
                BuildDown(childNode, inputKeeper);
            }
        }

        private Node<char> AttachChild(Node<char> parentNode, char childValue, bool isLeft)
        {
            if (!TrySaveToProcessed(childValue))
            {
                //in most cases I would return a result containing either value or error.
                //why exception here?
                //1. we save on allocation/copying result for each node.
                //2. the code using this method looks simpler
                //3. it's intented to be used inside class only. Public methods return result object.
                //So, exception here is just a convinient way to transfer an error
                throw new TreeBuildException(General.ErrorCode.Cycle);
            }

            var childNode = new Node<char>(childValue);

            if (isLeft)
            {
                parentNode.Left = childNode;
            }
            else
            {
                parentNode.Right = childNode;
            }

            return childNode;
        }

        private Node<char> AttachParent(char parentValue, Node<char> childNode, bool isLeft)
        {
            if (!TrySaveToProcessed(parentValue))
            {
                throw new TreeBuildException(General.ErrorCode.Cycle);
            }

            var newHead = new Node<char>(parentValue);

            if (isLeft)
            {
                newHead.Left = childNode;
            }
            else
            {
                newHead.Right = childNode;
            }

            return newHead;
        }

        private bool TrySaveToProcessed(char value)
        {
            //false if the element is already processed.
            return (processedValues ?? throw new NullReferenceException($"{nameof(processedValues)}"))
                .Add(value);
        }

        private void InitNewBuild()
        {
            processedValues = new HashSet<char>();
            firstRoot = null;
            rootsCount = 0;
        }

        private bool IsFirstLeft(char first, char second) => first < second;
    }
}
