namespace LibraryApi.Application.DTOs.Books;

public record BookResponse(Guid Id, string Title, string Author, string Isbn, int PublishedYear, string Genre, bool IsAvailable, DateTime CreatedAt, DateTime? UpdatedAt);
