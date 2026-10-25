namespace BuildingBlocks.Exceptions
{
    public class InternalServerException : SystemException
    {
        public string? Details { get; }

        public InternalServerException(string message, string? details = null)
            : base(message)
        {
            Details = details;
        }
    }
}
