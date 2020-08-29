using ImplCore.Tree;
using System.Collections.Generic;

namespace ImplCore.Input
{
    internal class InputParser
    {
        private const string itemDelimiter = " ";

        public OperationResult<List<InputItem>> Parse(string input)
        {
            string[] rawItems = input.Split(itemDelimiter);
            var result = new List<InputItem>(rawItems.Length);

            foreach(string rawItem in rawItems)
            {
                if (TryParseInputItem(rawItem, out InputItem parsedItem))
                {
                    result.Add(parsedItem);
                }
                else
                {
                    return OperationResult<List<InputItem>>.Error();
                }
            }

            return OperationResult<List<InputItem>>.Success(result);
        }

        private bool TryParseInputItem(string inputItem, out InputItem result)
        {
            if (!ValidateItemInput(inputItem))
            {
                result = default;
                return false;
            }

            char parent = GetFirstItem(inputItem);
            char child = GetSecondItem(inputItem);

            result = new InputItem(parent, child);

            return true;
        }

        private bool ValidateItemInput(string inputItem)
        {
            //another obvious option is Regex, but I believe manual checking is better here.
            if (inputItem.Length != 5)
            {
                return false;
            }

            if (inputItem[0] != '(' || inputItem[2] != ',' || inputItem[inputItem.Length - 1] != ')')
            {
                return false;
            }

            char parent = GetFirstItem(inputItem);
            char child = GetSecondItem(inputItem);

            if (!char.IsUpper(parent) || !char.IsUpper(child))
            {
                return false;
            }

            return true;
        }

        private char GetFirstItem(string inputItem) => inputItem[1];
        private char GetSecondItem(string inputItem) => inputItem[3];

        //private InputItem ValidateItemInputRegex(string inputItem)//TODO: delete
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
