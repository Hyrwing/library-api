using LibraryApi.Application.Interfaces;
using LibraryApi.Domain.Entities;
using LibraryApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _db;
    public BookRepository(LibraryDbContext db) => _db = db;

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Books.FindAsync([id], ct);

    public async Task<Book?> GetByIdWithLoansAsync(Guid id, CancellationToken ct)
        => await _db.Books.Include(b => b.Loans).FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct)
        => await _db.Books.FirstOrDefaultAsync(b => b.Isbn == isbn, ct);

    public async Task<(IReadOnlyList<Book> Items, int TotalCount)> ListAsync(
        string? search, string? title, string? author,
        string? genre, bool? available,
        int page, int pageSize, CancellationToken ct)
    {
        var query = _db.Books.Include(b => b.Loans).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(b => b.Title.Contains(title));
        if (!string.IsNullOrWhiteSpace(author))
            query = query.Where(b => b.Author.Contains(author));
        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(b => b.Genre == genre);
        if (available.HasValue)
            query = available.Value
                ? query.Where(b => !b.Loans.Any(l => l.ReturnDate == null))
                : query.Where(b => b.Loans.Any(l => l.ReturnDate == null));

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(b => b.Title).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, totalCount);
    }

    public async Task AddAsync(Book book, CancellationToken ct)
    { _db.Books.Add(book); await _db.SaveChangesAsync(ct); }

    public async Task UpdateAsync(Book book, CancellationToken ct)
        => await _db.SaveChangesAsync(ct);

    public async Task DeleteAsync(Book book, CancellationToken ct)
    { _db.Books.Remove(book); await _db.SaveChangesAsync(ct); }

    public async Task<bool> HasLoansAsync(Guid bookId, CancellationToken ct)
        => await _db.Loans.AnyAsync(l => l.BookId == bookId, ct);
}
