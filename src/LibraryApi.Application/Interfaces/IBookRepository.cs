using LibraryApi.Domain.Entities;

namespace LibraryApi.Application.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Book?> GetByIdWithLoansAsync(Guid id, CancellationToken ct = default);
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default);
    Task<(IReadOnlyList<Book> Items, int TotalCount)> ListAsync(
        string? search, string? title, string? author,
        string? genre, bool? available,
        int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Book book, CancellationToken ct = default);
    Task UpdateAsync(Book book, CancellationToken ct = default);
    Task DeleteAsync(Book book, CancellationToken ct = default);
    Task<bool> HasLoansAsync(Guid bookId, CancellationToken ct = default);
}
