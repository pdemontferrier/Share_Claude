
namespace BatchStockRelease.A_Domain.Common.Exceptions
{
    /// <summary>Exception levée si un accès à la base échoue.</summary>
    public class Ex_Infrastructure : Exception
    {
        public string? CallChain { get; }
        public string? ErrorId { get; }
        public string? SecondDictionaryKey { get; }

        public Ex_Infrastructure() { }
        public Ex_Infrastructure(string message) : base(message) { }
        public Ex_Infrastructure(string message, Exception innerException) : base(message, innerException) { }
        public Ex_Infrastructure(string callChain, string errorId, string secondDictionaryKey, Exception? innerException = null)
            : base(secondDictionaryKey, innerException)
        {
            CallChain = callChain;
            ErrorId = errorId;
            SecondDictionaryKey = secondDictionaryKey;
        }
    }
}