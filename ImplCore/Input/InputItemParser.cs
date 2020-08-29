namespace ImplCore.Input
{
    internal class CharInputItemParser : IInputItemParser<char>
    {
        public bool TryExtractItem(string input, out char result)
        {
            if (!char.TryParse(input, out result))
            {
                return false;
            }

            return char.IsUpper(result);
        }
    }
}
