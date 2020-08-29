namespace ImplCore.Input
{
    public readonly struct InputItem<T> where T: notnull
    {
        public T Parent { get; }
        public T Child { get; }

        public InputItem(T parent, T child)
        {
            Parent = parent;
            Child = child;
        }
    }
}
