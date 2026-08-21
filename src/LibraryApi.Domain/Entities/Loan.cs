using LibraryApi.Domain.Exceptions;

namespace LibraryApi.Domain.Entities;

public class Loan : BaseEntity
{
    public Guid BookId { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    public string BorrowerEmail { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; set; }

    public Book Book { get; set; } = null!;

    public bool IsActive => ReturnDate == null;

    private Loan()
    {
    }

    public Loan(Guid bookId, string borrowerName, string borrowerEmail, DateTime dueDate)
    {
        BookId = bookId;
        BorrowerName = borrowerName;
        BorrowerEmail = borrowerEmail;
        LoanDate = DateTime.UtcNow;
        DueDate = dueDate;

        if (DueDate <= LoanDate)
            throw new DomainException("Due date must be after the loan date.");
    }

    public void Return()
    {
        if (!IsActive)
            throw new LoanAlreadyReturnedException(Id);

        ReturnDate = DateTime.UtcNow;
    }
}
