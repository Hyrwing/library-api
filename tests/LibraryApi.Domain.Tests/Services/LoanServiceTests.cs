using FluentAssertions;
using LibraryApi.Application.DTOs.Loans;
using LibraryApi.Application.Interfaces;
using LibraryApi.Application.Services;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace LibraryApi.Domain.Tests.Services;

public class LoanServiceTests
{
    private readonly Mock<ILoanRepository> _loanRepo = new();
    private readonly Mock<IBookRepository> _bookRepo = new();
    private readonly Mock<ILogger<LoanService>> _logger = new();
    private readonly LoanService _sut;

    public LoanServiceTests() { _sut = new LoanService(_loanRepo.Object, _bookRepo.Object, _logger.Object); }

    [Fact]
    public async Task CreateAsync_WhenBookNotFound_ThrowsNotFoundException()
    {
        _bookRepo.Setup(r => r.GetByIdWithLoansAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Book?)null);
        var request = new CreateLoanRequest(Guid.NewGuid(), "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        var act = () => _sut.CreateAsync(request, default);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WhenBookAlreadyLoaned_ThrowsBookAlreadyLoanedException()
    {
        var book = new Book { Id = Guid.NewGuid() };
        book.Loans.Add(new Loan(book.Id, "Prev", "prev@t.com", DateTime.UtcNow.AddDays(14)));
        _bookRepo.Setup(r => r.GetByIdWithLoansAsync(book.Id, default)).ReturnsAsync(book);
        var request = new CreateLoanRequest(book.Id, "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        var act = () => _sut.CreateAsync(request, default);
        await act.Should().ThrowAsync<BookAlreadyLoanedException>();
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ReturnsLoanResponse()
    {
        var book = new Book { Id = Guid.NewGuid(), Title = "Test Book" };
        _bookRepo.Setup(r => r.GetByIdWithLoansAsync(book.Id, default)).ReturnsAsync(book);
        var request = new CreateLoanRequest(book.Id, "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        var result = await _sut.CreateAsync(request, default);
        result.BorrowerName.Should().Be("John");
        result.IsActive.Should().BeTrue();
        _loanRepo.Verify(r => r.AddAsync(It.IsAny<Loan>(), default), Times.Once);
    }

    [Fact]
    public async Task ReturnAsync_WhenAlreadyReturned_ThrowsLoanAlreadyReturnedException()
    {
        var loan = new Loan(Guid.NewGuid(), "John", "john@t.com", DateTime.UtcNow.AddDays(14));
        loan.Return();
        loan.Book = new Book();
        _loanRepo.Setup(r => r.GetByIdWithBookAsync(loan.Id, default)).ReturnsAsync(loan);
        var act = () => _sut.ReturnAsync(loan.Id, default);
        await act.Should().ThrowAsync<LoanAlreadyReturnedException>();
    }
}
