namespace LibraryApi.Application.DTOs.Books;

public record BookListResponse(IReadOnlyList<BookResponse> Items, int TotalCount, int Page, int PageSize);
