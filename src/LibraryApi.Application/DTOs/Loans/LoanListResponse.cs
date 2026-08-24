namespace LibraryApi.Application.DTOs.Loans;

public record LoanListResponse(IReadOnlyList<LoanResponse> Items, int TotalCount, int Page, int PageSize);
