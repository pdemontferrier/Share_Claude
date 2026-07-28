
namespace LeitTemporImport.A_Domain.Common.Exceptions
{
    /// <summary>Exception levée si un accès à la base échoue.</summary>
    public class Ex_Infrastructure : Exception
    {
        public string? CallChain { get; }
        public string? ErrorId { get; }
        public string? ErrorException { get; }

        public Ex_Infrastructure() { }
        public Ex_Infrastructure(string errorException) : base(errorException) { }
        public Ex_Infrastructure(string errorException, Exception innerException) : base(errorException, innerException) { }
        public Ex_Infrastructure(string callChain, string errorId, string errorException, Exception? innerException = null)
            : base(errorException, innerException)
        {
            CallChain = callChain;
            ErrorId = errorId;
            ErrorException = errorException;
        }
    }
}