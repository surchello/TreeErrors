namespace ImplCore.Tree
{
    public class Node<T> where T: notnull
    {
        public Node<T>? Right { get; set; }
        public Node<T>? Left { get; set; }
        public T Value { get; }

        public Node(T value)
        {
            Value = value;
        }
    }
}
