
namespace LeitTemporImport.A_Domain.Common.Exceptions
{
    /// <summary>Exception levée si une règle métier est violée.</summary>
    public class Ex_Business : Exception
    {
        public string? CallChain { get; }
        public string? ErrorId { get; }
        public string? ErrorException { get; }

        public Ex_Business() { }
        public Ex_Business(string errorException) : base(errorException) { }
        public Ex_Business(string errorException, Exception innerException) : base(errorException, innerException) { }
        public Ex_Business(string callChain, string errorId, string errorException, Exception? innerException = null)
            : base(errorException, innerException)
        {
            CallChain = callChain;
            ErrorId = errorId;
            ErrorException = errorException;
        }
    }
}