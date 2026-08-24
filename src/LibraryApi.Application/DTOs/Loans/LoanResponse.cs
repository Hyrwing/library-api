namespace LibraryApi.Application.DTOs.Loans;

public record LoanResponse(Guid Id, Guid BookId, string BookTitle, string BorrowerName, string BorrowerEmail, DateTime LoanDate, DateTime DueDate, DateTime? ReturnDate, bool IsActive);
