using ImplCore.General;

namespace ImplCore.Input
{
    internal readonly struct AddItemResult
    {
        public bool IsAdded { get; }
        public ErrorCode? ErrorCode { get; }

        private AddItemResult(bool isAdded, ErrorCode? errorCode)
        {
            IsAdded = isAdded;
            ErrorCode = errorCode;
        }

        public static AddItemResult Added() => new AddItemResult(true, null);

        public static AddItemResult NotAdded() => new AddItemResult(false, null);

        public static AddItemResult Error(ErrorCode errorCode) => new AddItemResult(false, errorCode);
    }
}
