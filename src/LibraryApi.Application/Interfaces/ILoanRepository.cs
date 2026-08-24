using LibraryApi.Domain.Entities;

namespace LibraryApi.Application.Interfaces;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Loan?> GetByIdWithBookAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Loan> Items, int TotalCount)> ListAsync(
        string? search, Guid? bookId, bool? active,
        int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Loan loan, CancellationToken ct = default);
    Task UpdateAsync(Loan loan, CancellationToken ct = default);
}
