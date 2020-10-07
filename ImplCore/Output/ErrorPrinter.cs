using System;
using System.Collections.Generic;
using ImplCore.General;

namespace ImplCore.Output
{
    internal class ErrorPrinter
    {
        Dictionary<ErrorCode, string> displayErrors;

        public ErrorPrinter()
        {
            displayErrors = new Dictionary<ErrorCode, string> {
                { ErrorCode.InvalidInput, "E1" },
                { ErrorCode.DuplicatePair, "E2" },
                { ErrorCode.TooManyChildren, "E3" },
                { ErrorCode.Cycle, "E4" },
                { ErrorCode.MultipleRoots, "E5" },
            };
        }

        public void Print(ErrorCode errorCode)
        {
            if (displayErrors.TryGetValue(errorCode, out string? displayError))
            {
                throw new ArgumentException($"Unsupported error code: {errorCode}");
            }

            Console.WriteLine(displayError);
        }
    }
}
