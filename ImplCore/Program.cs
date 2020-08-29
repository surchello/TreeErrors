using ImplCore.General;
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
                if (!parseResult.IsSuccess)
                {
                    PrintError(parseResult.ErrorCode);
                    return;
                }

                var inputKeeperResult = InputKeeper.Create(parseResult.Result!);
                if (!inputKeeperResult.IsSuccess)
                {
                    PrintError(inputKeeperResult.ErrorCode);
                    return;
                }

                var treeResult = new TreeBuilder().Build(inputKeeperResult.Result!);
                if (!treeResult.IsSuccess)
                {
                    PrintError(treeResult.ErrorCode);
                    return;
                }

                string printResult = new TreePrinter().ToSExpression(treeResult.Result!);

                Console.WriteLine(printResult);
                Console.WriteLine(new TreePrinter().ToSExpressionIterative(treeResult.Result!));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
        }

        static void PrintError(ErrorCode errorCode) => new ErrorPrinter().Print(errorCode);
    }
}
