using LibraryApi.Api.Infrastructure;
using LibraryApi.Application.DTOs.Books;
using LibraryApi.Application.Services;

namespace LibraryApi.Api.Endpoints;

public static class BooksApi
{
    public static RouteGroupBuilder MapBooks(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/books").WithTags("Books");
        group.MapGet("/", ListBooks);
        group.MapGet("/{id:guid}", GetBook);
        group.MapPost("/", CreateBook).AddEndpointFilter<ValidationFilter<CreateBookRequest>>();
        group.MapPut("/{id:guid}", UpdateBook).AddEndpointFilter<ValidationFilter<UpdateBookRequest>>();
        group.MapDelete("/{id:guid}", DeleteBook);
        return group;
    }

    private static async Task<IResult> ListBooks(
        string? search, string? title, string? author,
        string? genre, bool? available,
        int? page, int? pageSize,
        BookService bookService, CancellationToken ct)
    {
        var result = await bookService.ListAsync(search, title, author, genre, available, page ?? 1, pageSize ?? 10, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetBook(Guid id, BookService bookService, CancellationToken ct)
    {
        var result = await bookService.GetByIdAsync(id, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> CreateBook(CreateBookRequest request, BookService bookService, CancellationToken ct)
    {
        var result = await bookService.CreateAsync(request, ct);
        return TypedResults.Created($"/api/books/{result.Id}", result);
    }

    private static async Task<IResult> UpdateBook(Guid id, UpdateBookRequest request, BookService bookService, CancellationToken ct)
    {
        var result = await bookService.UpdateAsync(id, request, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeleteBook(Guid id, BookService bookService, CancellationToken ct)
    {
        await bookService.DeleteAsync(id, ct);
        return TypedResults.NoContent();
    }
}
