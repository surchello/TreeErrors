using System.Collections.Generic;
using ImplCore.General;
using ImplCore.Tree;

namespace ImplCore.Input
{
    internal class InputParser<T> where T : notnull
    {
        private readonly IInputItemParser<T> inputItemParser;

        public InputParser(IInputItemParser<T> inputItemParser)
        {
            this.inputItemParser = inputItemParser;
        }

        public OperationResult<List<InputItem<T>>> Parse(string input)
        {
            string[] rawItems = input.Split(" ");
            var result = new List<InputItem<T>>(rawItems.Length);

            foreach(string rawItem in rawItems)
            {
                if (TryParseInputItem(rawItem, out InputItem<T> parsedItem))
                {
                    result.Add(parsedItem);
                }
                else
                {
                    return OperationResult<List<InputItem<T>>>.Error(ErrorCode.InvalidInput);
                }
            }

            return OperationResult<List<InputItem<T>>>.Success(result);
        }

        private bool TryParseInputItem(string inputItem, out InputItem<T> result)
        {
            result = default;

            if (inputItem[0] != '(' || inputItem[inputItem.Length - 1] != ')')
            {
                return false;
            }

            int separatorPosition = inputItem.IndexOf(',');
            if (separatorPosition < 0)
            {
                return false;
            }

            string firstItem = inputItem.Substring(0, separatorPosition);
            string secondItem = inputItem.Substring(separatorPosition + 1, (inputItem.Length - 1) - separatorPosition);

            if (!inputItemParser.TryExtractItem(firstItem, out T first))
            {
                return false;
            }

            if (!inputItemParser.TryExtractItem(secondItem, out T second))
            {
                return false;
            }

            result = new InputItem<T>(first, second);

            return true;
        }

        //private InputItem ValidateItemInputRegex(string inputItem)
        //{
        //    var parseResult = Regex.Match(inputItem, @"^\(([A-Z]{1}),([A-Z]{1})\)$");
        //    if (!parseResult.Success)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    if (parseResult.Groups.Count != 3)
        //    {
        //        throw new Exception($"Wrong parser template. Groups found: {parseResult.Groups.Count}.");
        //    }

        //    return new InputItem(char.Parse(parseResult.Groups[1].Value), char.Parse(parseResult.Groups[2].Value));
        //}
    }
}
