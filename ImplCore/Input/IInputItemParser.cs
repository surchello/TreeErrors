namespace ImplCore.Input
{
    internal interface IInputItemParser<T> where T: notnull
    {
        bool TryExtractItem(string input, out T result);
    }
}
