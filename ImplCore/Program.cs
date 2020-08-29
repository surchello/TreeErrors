using ImplCore.Input;
using ImplCore.Output;
using ImplCore.Tree;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ImplCore
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                var s = "(A,B)";
                var pattern = @"^\((?<parent>[A-Z]{1}),(?<child>[A-Z]{1})\)$";

                var t = Regex.Match(s, pattern);
            }
            //=====

            string input = "(B,D) (D,E) (A,B) (C,F) (E,G) (A,C)";
            PrintSExpression(input);

            Console.ReadLine();
        }

        static void PrintSExpression(string input)
        {
            try
            {
                var parseResult = new InputParser().Parse(input);
                if (parseResult.IsError)
                {
                    new ErrorPrinter().Print(General.ErrorCode.InvalidInput);
                    return;
                }

                var inputKeeper = InputKeeper.Create(parseResult.Value!);
                var treeResult = new TreeBuilder().Build(inputKeeper);

                var printResult = new TreePrinter().ToSExpression(treeResult);

                Console.WriteLine(printResult);
                Console.WriteLine(new TreePrinter().ToSExpressionIterative(treeResult));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
        }
    }
}
