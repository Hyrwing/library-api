using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LibraryApi.Api.Tests.Infrastructure;
using LibraryApi.Application.DTOs.Books;

namespace LibraryApi.Api.Tests.Endpoints;

public class BooksApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksApiTests(CustomWebApplicationFactory factory) { _client = factory.CreateClient(); }

    [Fact]
    public async Task GetBooks_ReturnsOk_WithSeededData()
    {
        var response = await _client.GetAsync("/api/books");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BookListResponse>();
        result.Should().NotBeNull();
        result!.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateBook_WithValidData_ReturnsCreated()
    {
        var request = new CreateBookRequest("Test Book", "Test Author", "1234567890123", 2020, "Fiction");
        var response = await _client.PostAsJsonAsync("/api/books", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<BookResponse>();
        result!.Title.Should().Be("Test Book");
    }

    [Fact]
    public async Task CreateBook_WithInvalidData_ReturnsBadRequest()
    {
        var request = new CreateBookRequest("", "", "invalid", 999, "");
        var response = await _client.PostAsJsonAsync("/api/books", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBook_DuplicateIsbn_ReturnsConflict()
    {
        var request = new CreateBookRequest("Book A", "Author A", "1111111111111", 2020, "Fiction");
        await _client.PostAsJsonAsync("/api/books", request);
        var request2 = new CreateBookRequest("Book B", "Author B", "1111111111111", 2021, "Fiction");
        var response = await _client.PostAsJsonAsync("/api/books", request2);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetBook_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/books/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBooks_WithSearch_FiltersResults()
    {
        var response = await _client.GetAsync("/api/books?search=Tolkien");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BookListResponse>();
        result!.Items.Should().AllSatisfy(b =>
            (b.Title + b.Author).Should().ContainEquivalentOf("Tolkien"));
    }

    [Fact]
    public async Task UpdateBook_WithValidData_ReturnsOk()
    {
        var create = new CreateBookRequest("Original", "Author", "9999999999999", 2020, "Fiction");
        var createResponse = await _client.PostAsJsonAsync("/api/books", create);
        var created = await createResponse.Content.ReadFromJsonAsync<BookResponse>();
        var update = new UpdateBookRequest("Updated", "Author", "9999999999999", 2020, "Fiction");
        var response = await _client.PutAsJsonAsync($"/api/books/{created!.Id}", update);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BookResponse>();
        result!.Title.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteBook_NoLoans_ReturnsNoContent()
    {
        var create = new CreateBookRequest("ToDelete", "Author", "8888888888888", 2020, "Fiction");
        var createResponse = await _client.PostAsJsonAsync("/api/books", create);
        var created = await createResponse.Content.ReadFromJsonAsync<BookResponse>();
        var response = await _client.DeleteAsync($"/api/books/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
