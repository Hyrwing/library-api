namespace LibraryApi.Application.DTOs.Books;

public record UpdateBookRequest(string Title, string Author, string Isbn, int PublishedYear, string Genre);
