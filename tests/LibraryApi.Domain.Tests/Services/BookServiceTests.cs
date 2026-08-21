using FluentAssertions;
using LibraryApi.Application.DTOs.Books;
using LibraryApi.Application.Interfaces;
using LibraryApi.Application.Services;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace LibraryApi.Domain.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepo = new();
    private readonly Mock<ILogger<BookService>> _logger = new();
    private readonly BookService _sut;

    public BookServiceTests() { _sut = new BookService(_bookRepo.Object, _logger.Object); }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ThrowsNotFoundException()
    {
        _bookRepo.Setup(r => r.GetByIdWithLoansAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Book?)null);
        var act = () => _sut.GetByIdAsync(Guid.NewGuid(), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WhenDuplicateIsbn_ThrowsDomainException()
    {
        _bookRepo.Setup(r => r.GetByIsbnAsync("1234567890", default)).ReturnsAsync(new Book { Isbn = "1234567890" });
        var request = new CreateBookRequest("Title", "Author", "1234567890", 2020, "Fiction");
        var act = () => _sut.CreateAsync(request, default);
        await act.Should().ThrowAsync<DomainException>().WithMessage("*ISBN*already exists*");
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ReturnsBookResponse()
    {
        _bookRepo.Setup(r => r.GetByIsbnAsync(It.IsAny<string>(), default)).ReturnsAsync((Book?)null);
        var request = new CreateBookRequest("Title", "Author", "1234567890", 2020, "Fiction");
        var result = await _sut.CreateAsync(request, default);
        result.Title.Should().Be("Title");
        result.IsAvailable.Should().BeTrue();
        _bookRepo.Verify(r => r.AddAsync(It.IsAny<Book>(), default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenHasLoans_ThrowsDomainException()
    {
        var bookId = Guid.NewGuid();
        _bookRepo.Setup(r => r.GetByIdAsync(bookId, default)).ReturnsAsync(new Book { Id = bookId });
        _bookRepo.Setup(r => r.HasLoansAsync(bookId, default)).ReturnsAsync(true);
        var act = () => _sut.DeleteAsync(bookId, default);
        await act.Should().ThrowAsync<DomainException>().WithMessage("*loan history*");
    }
}
