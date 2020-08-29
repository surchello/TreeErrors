using System;
using System.Diagnostics.CodeAnalysis;
using ImplCore.General;

namespace ImplCore.Tree
{
    public class OperationResult<T>
    {
        [AllowNull]
        private readonly T result;
        private readonly ErrorCode? errorCode;

        [MaybeNull]
        public T Result
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new Exception($"Failed to return a result since there is an error: {ErrorCode}.");
                }

                return result;
            }
        }

        public ErrorCode ErrorCode
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new Exception($"Failed to return errorCode since there is no error.");
                }

                return errorCode!.Value;
            }
        }

        public bool IsSuccess => !errorCode.HasValue;

        private OperationResult([AllowNull]T result, ErrorCode? errorCode)
        {
            this.result = result;
            this.errorCode = errorCode;
        }

        public static OperationResult<T> Success(T result) => new OperationResult<T>(result, null);

        public static OperationResult<T> Error(ErrorCode errorCode) => new OperationResult<T>(default, errorCode);
    }
}
