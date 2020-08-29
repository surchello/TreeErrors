using System;
using System.Diagnostics.CodeAnalysis;

namespace ImplCore.Tree
{
    internal class OperationResult<T>
    {
        [AllowNull]
        private readonly T result;
        
        [MaybeNull]
        public T Result
        {
            get
            {
                if (IsError)
                {
                    throw new Exception($"Failed to return a result since there is an error.");
                }

                return result;
            }
        }

        public bool IsError { get; }

        private OperationResult([AllowNull]T result, bool isError)
        {
            this.result = result;
            IsError = isError;
        }

        public static OperationResult<T> Success(T result) => new OperationResult<T>(result, false);

        public static OperationResult<T> Error() => new OperationResult<T>(default, true);
    }
}
