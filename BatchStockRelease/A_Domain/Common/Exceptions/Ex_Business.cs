
namespace BatchStockRelease.A_Domain.Common.Exceptions
{
    /// <summary>Exception levée si une règle métier est violée.</summary>
    public class Ex_Business : Exception
    {
        public string? CallChain { get; }
        public string? ErrorId { get; }
        public string? SecondDictionaryKey { get; }

        public Ex_Business() { }
        public Ex_Business(string message) : base(message) { }
        public Ex_Business(string message, Exception innerException) : base(message, innerException) { }
        public Ex_Business(string callChain, string errorId, string secondDictionaryKey, Exception? innerException = null)
            : base(secondDictionaryKey, innerException)
        {
            CallChain = callChain;
            ErrorId = errorId;
            SecondDictionaryKey = secondDictionaryKey;
        }
    }
}