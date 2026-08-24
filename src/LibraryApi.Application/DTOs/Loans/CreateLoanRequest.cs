namespace LibraryApi.Application.DTOs.Loans;

public record CreateLoanRequest(Guid BookId, string BorrowerName, string BorrowerEmail, DateTime DueDate);
