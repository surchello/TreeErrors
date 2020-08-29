using ImplCore.Tree;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImplCore.Output
{
    internal class TreePrinter
    {
        public string ToSExpression(Node<char>? head)
        {
            if (head == null)
            {
                return "";
            }

            var left = ToSExpression(head.Left);
            var right = ToSExpression(head.Right);

            return $"({head.Value}{left}{right})";
        }

        public string ToSExpressionIterative(Node<char>? head)
        {
            var res = new StringBuilder();
            var stack = new Stack<StackItem>();

            int startLevel = 1;
            int level = startLevel;

            while (head != null)
            {
                res.Append("(" + head.Value);

                if (head.Right != null)
                {
                    stack.Push(new StackItem(head.Right, level + 1));
                }

                if (head.Left != null)
                {
                    head = head.Left;
                    level++;
                }
                else if (stack.Count != 0)
                {
                    var stackItem = stack.Pop();

                    AddCloseBrackets(res, level, stackItem.Level);

                    head = stackItem.Node;
                    level = stackItem.Level;
                }
                else
                {
                    AddCloseBrackets(res, level, startLevel);
                    head = null;
                }
            }

            return res.ToString();
        }

        private void AddCloseBrackets(StringBuilder stringBuilder, int currentLevel, int closeLevel)
        {
            var levelDiff = currentLevel - closeLevel;
            stringBuilder.Append(new string(')', levelDiff + 1));//one own closing bracket + levelDiff to close parents
        }

        private readonly struct StackItem
        {
            public Node<char> Node { get; }
            public int Level { get; }

            public StackItem(Node<char> node, int level)
            {
                Node = node;
                Level = level;
            }
        }

        //public string ToSExpression(Node<char>? head)
        //{
        //    string res = "";

        //    var stack = new Stack<Node<char>>();
        //    while(head != null || stack.Count != 0)
        //    {
        //        while(head != null)
        //        {
        //            stack.Push(head);
        //            //print
        //            head = head.Left;
        //        }

        //        head = stack.Pop();

        //        head = head.Right;

        //    }

        //    return res;
        //}
    }
}
