using LibraryApi.Application.DTOs.Books;
using LibraryApi.Application.Interfaces;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace LibraryApi.Application.Services;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<BookService> _logger;

    public BookService(IBookRepository bookRepository, ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task<BookResponse> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var book = await _bookRepository.GetByIdWithLoansAsync(id, ct)
            ?? throw new NotFoundException(nameof(Book), id);
        return MapToResponse(book);
    }

    public async Task<BookListResponse> ListAsync(
        string? search, string? title, string? author,
        string? genre, bool? available,
        int page, int pageSize, CancellationToken ct)
    {
        var (items, totalCount) = await _bookRepository.ListAsync(search, title, author, genre, available, page, pageSize, ct);
        return new BookListResponse(items.Select(MapToResponse).ToList(), totalCount, page, pageSize);
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken ct)
    {
        var existing = await _bookRepository.GetByIsbnAsync(request.Isbn, ct);
        if (existing is not null)
            throw new DomainException($"A book with ISBN '{request.Isbn}' already exists.");

        var book = new Book
        {
            Title = request.Title, Author = request.Author, Isbn = request.Isbn,
            PublishedYear = request.PublishedYear, Genre = request.Genre
        };
        await _bookRepository.AddAsync(book, ct);
        _logger.LogInformation("Book created: {BookId} - {Title}", book.Id, book.Title);
        return MapToResponse(book);
    }

    public async Task<BookResponse> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken ct)
    {
        var book = await _bookRepository.GetByIdWithLoansAsync(id, ct)
            ?? throw new NotFoundException(nameof(Book), id);

        var existingWithIsbn = await _bookRepository.GetByIsbnAsync(request.Isbn, ct);
        if (existingWithIsbn is not null && existingWithIsbn.Id != id)
            throw new DomainException($"A book with ISBN '{request.Isbn}' already exists.");

        book.Title = request.Title; book.Author = request.Author; book.Isbn = request.Isbn;
        book.PublishedYear = request.PublishedYear; book.Genre = request.Genre;
        book.UpdatedAt = DateTime.UtcNow;

        await _bookRepository.UpdateAsync(book, ct);
        _logger.LogInformation("Book updated: {BookId}", book.Id);
        return MapToResponse(book);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var book = await _bookRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Book), id);
        if (await _bookRepository.HasLoansAsync(id, ct))
            throw new DomainException($"Cannot delete book '{id}' because it has loan history.");
        await _bookRepository.DeleteAsync(book, ct);
        _logger.LogInformation("Book deleted: {BookId}", id);
    }

    private static BookResponse MapToResponse(Book book) => new(
        book.Id, book.Title, book.Author, book.Isbn,
        book.PublishedYear, book.Genre, !book.HasActiveLoan,
        book.CreatedAt, book.UpdatedAt);
}
