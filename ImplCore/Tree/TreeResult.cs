using ImplCore.General;
using System;

namespace ImplCore.Tree
{
    public class TreeResult<T> where T: notnull
    {
        private readonly Node<T>? result;

        public Node<T>? Result
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

        public ErrorCode? ErrorCode { get; }

        public bool IsSuccess => !ErrorCode.HasValue;

        private TreeResult(Node<T>? result, ErrorCode? errorCode)
        {
            this.result = result;
            ErrorCode = errorCode;
        }

        public static TreeResult<T> Success(Node<T>? result) => new TreeResult<T>(result, null);

        public static TreeResult<T> Error(ErrorCode errorCode) => new TreeResult<T>(null, errorCode);
    }
}
