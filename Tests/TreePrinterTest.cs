using System.Collections.Generic;
using Xunit;
using ImplCore.Tree;
using ImplCore.Output;

namespace Tests
{
    public class TreePrinterTest
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void Print(Node<char>? head, string expected)
        {
            var result = new TreePrinter<char>().ToSExpression(head);
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void PrintIterative(Node<char>? head, string expected)
        {
            var result = new TreePrinter<char>().ToSExpressionIterative(head);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object?[]> Data()
        {
            var data1 = new Node<char>('A')
            {
                Left = new Node<char>('B')
            };

            var data2 = new Node<char>('A')
            {
                Right = new Node<char>('B')
            };

            var data3 = new Node<char>('A')
            {
                Left = new Node<char>('B'),
                Right = new Node<char>('C')
            };

            var data4 = new Node<char>('A')
            {
                Left = new Node<char>('B')
                {
                    Left = new Node<char>('C')
                }
            };

            var data5 = new Node<char>('A')
            {
                Right = new Node<char>('B')
                {
                    Right = new Node<char>('C')
                }
            };

            var data6 = new Node<char>('A')
            {
                Left = new Node<char>('B')
                {
                    Left = new Node<char>('D'),
                    Right = new Node<char>('E')
                },
                Right = new Node<char>('C')
                {
                    Left = new Node<char>('G'),
                    Right = new Node<char>('F')
                }
            };

            var data7 = new Node<char>('A')
            {
                Left = new Node<char>('B')
                {
                    Left = new Node<char>('C')
                },
                Right = new Node<char>('D')
                {
                    Right = new Node<char>('E')
                }
            };

            return new List<object?[]>
            {
                new object?[] { null, "" },
                new object?[] { new Node<char>('A'), "(A)" },
                new object?[] { data1, "(A(B))" },
                new object?[] { data2, "(A(B))" },
                new object?[] { data3, "(A(B)(C))" },
                new object?[] { data4, "(A(B(C)))" },
                new object?[] { data5, "(A(B(C)))" },
                new object?[] { data6, "(A(B(D)(E))(C(G)(F)))" },
                new object?[] { data7, "(A(B(C))(D(E)))" }
            };
        }
    }
}
