namespace LibraryApi.Domain.Exceptions;

public class BookAlreadyLoanedException : DomainException
{
    public BookAlreadyLoanedException(Guid bookId)
        : base($"Book '{bookId}' already has an active loan.")
    {
    }
}
