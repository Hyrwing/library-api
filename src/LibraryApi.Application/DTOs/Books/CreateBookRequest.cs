namespace LibraryApi.Application.DTOs.Books;

public record CreateBookRequest(string Title, string Author, string Isbn, int PublishedYear, string Genre);
