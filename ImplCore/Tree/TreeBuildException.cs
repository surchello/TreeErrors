using System;
using ImplCore.General;

namespace ImplCore.Tree
{
    public class TreeBuildException : Exception
    {
        public ErrorCode ErrorCode { get; }

        public TreeBuildException(ErrorCode errorCode)
            : base(errorCode.ToString())
        {
            ErrorCode = errorCode;
        }
    }
}
