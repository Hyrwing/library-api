namespace LibraryApi.Domain.Exceptions;

public class LoanAlreadyReturnedException : DomainException
{
    public LoanAlreadyReturnedException(Guid loanId)
        : base($"Loan '{loanId}' has already been returned.")
    {
    }
}
