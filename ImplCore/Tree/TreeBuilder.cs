using System;
using System.Collections.Generic;
using ImplCore.General;
using ImplCore.Input;

namespace ImplCore.Tree
{
    public class TreeBuilder<T> where T : notnull, IEquatable<T>, IComparable<T>
    {
        private HashSet<T>? processedValues;
        private Node<T>? firstRoot;
        private int rootsCount;

        public OperationResult<Node<T>?> Build(IInputKeeper<T> inputKeeper)
        {
            InitNewBuild();
            try
            {
                Node<T>? head = GetStartHead(inputKeeper);
                if (head == null)
                {
                    return OperationResult<Node<T>?>.Success(head);
                }

                BuildDown(head, inputKeeper);
                BuildUp(head, inputKeeper);
            }
            catch (TreeBuildException ex)
            {
                return OperationResult<Node<T>?>.Error(ex.ErrorCode);
            }

            if (rootsCount > 1)
            {
                return OperationResult<Node<T>?>.Error(ErrorCode.MultipleRoots);
            }

            return OperationResult<Node<T>?>.Success(firstRoot);
        }

        private Node<T>? GetStartHead(IInputKeeper<T> inputKeeper)
        {
            InputItem<T>? firstItem = inputKeeper.GetFirstItem();
            if (!firstItem.HasValue)
            {
                return null;
            }

            var head = new Node<T>(firstItem.Value.Parent);
            if (!TrySaveToProcessed(head.Value))
            {
                throw new Exception($"First item is already processed.");
            }

            return head;
        }

        private void BuildUp(Node<T> head, IInputKeeper<T> inputKeeper)
        {
            if (!inputKeeper.TryGetPrimaryParent(head.Value, out T parent))
            {
                //this method could return a collection of roots but we don't need to know all of them
                //what we need to know is whether there is only one head. If yes we need a reference to that head.
                firstRoot ??= head;
                rootsCount++;

                return;
            }

            ProcessParent(parent, head, inputKeeper);
            BuildChildUp(head, inputKeeper);
        }

        private void BuildChildUp(Node<T> childNode, IInputKeeper<T> inputKeeper)
        {
            if (inputKeeper.TryGetAdditionalParents(childNode.Value, out IEnumerable<T> additionalParents))
            {
                //there're multiple heads but loop is still possible. We have to continue to determine what it is.
                foreach (var additionalParent in additionalParents)
                {
                    ProcessParent(additionalParent, childNode, inputKeeper);
                }
            }
        }

        private Node<T> ProcessParent(T parentValue, Node<T> oldHead, IInputKeeper<T> inputKeeper)
        {
            Node<T> newHead = AttachNewHeadDeep(parentValue, oldHead, inputKeeper);
            BuildUp(newHead, inputKeeper);

            return newHead;
        }

        private Node<T> AttachNewHeadDeep(T newHeadValue, Node<T> oldHead, IInputKeeper<T> inputKeeper)
        {
            bool isFirstFound = inputKeeper.TryGetFirstChild(newHeadValue, out T firstChild);
            if (!isFirstFound)
            {
                throw new Exception($"Children not found for value {newHeadValue}. At least one is expected: {oldHead.Value}");
            }

            Node<T> newHead;

            bool isSecondFound = inputKeeper.TryGetSecondChild(newHeadValue, out T secondChild);
            if (!isSecondFound)
            {
                newHead = AttachParent(newHeadValue, oldHead, isLeft: true);
                return newHead;
            }

            T otherChildValue = oldHead.Value.Equals(firstChild) ? firstChild : secondChild;
            bool isOldHeadLeft = IsFirstLeft(oldHead.Value, otherChildValue);

            newHead = AttachParent(newHeadValue, oldHead, isOldHeadLeft);
            Node<T> otherChildNode = AttachChild(newHead, otherChildValue, !isOldHeadLeft);

            BuildDown(otherChildNode, inputKeeper);

            return newHead;
        }

        private void BuildDown(Node<T> head, IInputKeeper<T> inputKeeper)
        {
            bool isFirstFound = inputKeeper.TryGetFirstChild(head.Value, out T firstChild);
            bool isSecondFound = inputKeeper.TryGetSecondChild(head.Value, out T secondChild);
            if (!isFirstFound && !isSecondFound)
            {
                return;
            }

            if (isFirstFound)
            {
                bool isLeft = !isSecondFound || IsFirstLeft(firstChild, secondChild);
                Node<T> childNode = AttachChild(head, firstChild, isLeft);
                BuildDown(childNode, inputKeeper);
                BuildChildUp(childNode, inputKeeper);
            }

            if (isSecondFound)
            {
                bool isLeft = !isFirstFound || IsFirstLeft(secondChild, firstChild);
                Node<T> childNode = AttachChild(head, secondChild, isLeft);
                BuildDown(childNode, inputKeeper);
                BuildChildUp(childNode, inputKeeper);
            }
        }

        private Node<T> AttachChild(Node<T> parentNode, T childValue, bool isLeft)
        {
            if (!TrySaveToProcessed(childValue))
            {
                //in most cases I would return a result containing either value or error.
                //why exception here?
                //1. we save on allocation/copying result for each node.
                //2. the code using this method looks simpler
                //3. it's intented to be private. Public methods return result object.
                //So, exception here is just a convinient way to transfer an error up the call stack
                throw new TreeBuildException(ErrorCode.Cycle);
            }

            var childNode = new Node<T>(childValue);

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

        private Node<T> AttachParent(T parentValue, Node<T> childNode, bool isLeft)
        {
            if (!TrySaveToProcessed(parentValue))
            {
                throw new TreeBuildException(ErrorCode.Cycle);
            }

            var newHead = new Node<T>(parentValue);

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

        private bool TrySaveToProcessed(T value)
        {
            //false if the element is already processed.
            return (processedValues ?? throw new NullReferenceException($"{nameof(processedValues)}"))
                .Add(value);
        }

        private void InitNewBuild()
        {
            processedValues = new HashSet<T>();
            firstRoot = null;
            rootsCount = 0;
        }

        private bool IsFirstLeft(T first, T second) => first.CompareTo(second) < 0;
    }
}
