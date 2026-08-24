using LibraryApi.Application.DTOs.Loans;
using LibraryApi.Application.Interfaces;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace LibraryApi.Application.Services;

public class LoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<LoanService> _logger;

    public LoanService(ILoanRepository loanRepository, IBookRepository bookRepository, ILogger<LoanService> logger)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task<LoanResponse> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var loan = await _loanRepository.GetByIdWithBookAsync(id, ct)
            ?? throw new NotFoundException(nameof(Loan), id);
        return MapToResponse(loan);
    }

    public async Task<LoanListResponse> ListAsync(
        string? search, Guid? bookId, bool? active,
        int page, int pageSize, CancellationToken ct)
    {
        var (items, totalCount) = await _loanRepository.ListAsync(search, bookId, active, page, pageSize, ct);
        return new LoanListResponse(items.Select(MapToResponse).ToList(), totalCount, page, pageSize);
    }

    public async Task<LoanResponse> CreateAsync(CreateLoanRequest request, CancellationToken ct)
    {
        var book = await _bookRepository.GetByIdWithLoansAsync(request.BookId, ct)
            ?? throw new NotFoundException(nameof(Book), request.BookId);

        if (!book.CanBeLoan())
            throw new BookAlreadyLoanedException(book.Id);

        var loan = new Loan(book.Id, request.BorrowerName, request.BorrowerEmail, request.DueDate);
        await _loanRepository.AddAsync(loan, ct);
        loan.Book = book;

        _logger.LogInformation("Loan created: {LoanId} for book {BookId} to {Borrower}", loan.Id, book.Id, loan.BorrowerName);
        return MapToResponse(loan);
    }

    public async Task<LoanResponse> ReturnAsync(Guid id, CancellationToken ct)
    {
        var loan = await _loanRepository.GetByIdWithBookAsync(id, ct)
            ?? throw new NotFoundException(nameof(Loan), id);
        loan.Return();
        await _loanRepository.UpdateAsync(loan, ct);
        _logger.LogInformation("Loan returned: {LoanId} for book {BookId}", loan.Id, loan.BookId);
        return MapToResponse(loan);
    }

    private static LoanResponse MapToResponse(Loan loan) => new(
        loan.Id, loan.BookId, loan.Book?.Title ?? string.Empty,
        loan.BorrowerName, loan.BorrowerEmail,
        loan.LoanDate, loan.DueDate, loan.ReturnDate, loan.IsActive);
}
