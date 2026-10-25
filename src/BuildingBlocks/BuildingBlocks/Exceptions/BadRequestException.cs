namespace BuildingBlocks.Exceptions
{
    public class BadRequestException : SystemException
    {
        public string? Details { get; }

        public BadRequestException(string message, string? details = null)
            : base(message)
        {
            Details = details;
        }
    }
}
