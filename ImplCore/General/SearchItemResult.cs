using System;
using System.Diagnostics.CodeAnalysis;

namespace ImplCore.Tree
{
    public struct SearchItemResult<T>
    {
        [AllowNull]
        private readonly T result;

        [MaybeNull]
        public T Result
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new Exception($"Failed to return a result since there is an error.");
                }

                return result;
            }
        }

        public bool IsSuccess { get; }

        private SearchItemResult([AllowNull]T result, bool isSuccess)
        {
            this.result = result;
            this.IsSuccess = isSuccess;
        }

        public static SearchItemResult<T> Success(T result) => new SearchItemResult<T>(result, true);

        public static SearchItemResult<T> Failed() => new SearchItemResult<T>(default, false);
    }
}
