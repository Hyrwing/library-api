using LibraryApi.Application.Interfaces;
using LibraryApi.Domain.Entities;
using LibraryApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LibraryDbContext _db;
    public LoanRepository(LibraryDbContext db) => _db = db;

    public async Task<Loan?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Loans.FindAsync([id], ct);

    public async Task<Loan?> GetByIdWithBookAsync(Guid id, CancellationToken ct)
        => await _db.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<(IReadOnlyList<Loan> Items, int TotalCount)> ListAsync(
        string? search, Guid? bookId, bool? active,
        int page, int pageSize, CancellationToken ct)
    {
        var query = _db.Loans.Include(l => l.Book).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(l => EF.Functions.Like(l.BorrowerName, $"%{search}%"));
        if (bookId.HasValue)
            query = query.Where(l => l.BookId == bookId.Value);
        if (active.HasValue)
            query = active.Value
                ? query.Where(l => l.ReturnDate == null)
                : query.Where(l => l.ReturnDate != null);

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderByDescending(l => l.LoanDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, totalCount);
    }

    public async Task AddAsync(Loan loan, CancellationToken ct)
    { 
        _db.Loans.Add(loan); 
        await _db.SaveChangesAsync(ct); 
    }

    public async Task UpdateAsync(Loan loan, CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
