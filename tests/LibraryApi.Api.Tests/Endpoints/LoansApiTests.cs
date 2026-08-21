using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LibraryApi.Api.Tests.Infrastructure;
using LibraryApi.Application.DTOs.Books;
using LibraryApi.Application.DTOs.Loans;

namespace LibraryApi.Api.Tests.Endpoints;

public class LoansApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoansApiTests(CustomWebApplicationFactory factory) { _client = factory.CreateClient(); }

    private async Task<BookResponse> CreateTestBookAsync()
    {
        var random = new Random();
        var isbn = random.NextInt64(1000000000000, 9999999999999).ToString();
        var request = new CreateBookRequest($"Test Book {isbn}", "Author", isbn, 2020, "Fiction");
        var response = await _client.PostAsJsonAsync("/api/books", request);
        return (await response.Content.ReadFromJsonAsync<BookResponse>())!;
    }

    [Fact]
    public async Task CreateLoan_WithValidData_ReturnsCreated()
    {
        var book = await CreateTestBookAsync();
        var request = new CreateLoanRequest(book.Id, "John Doe", "john@test.com", DateTime.UtcNow.AddDays(14));
        var response = await _client.PostAsJsonAsync("/api/loans", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<LoanResponse>();
        result!.BorrowerName.Should().Be("John Doe");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateLoan_BookAlreadyLoaned_ReturnsConflict()
    {
        var book = await CreateTestBookAsync();
        var loan1 = new CreateLoanRequest(book.Id, "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        await _client.PostAsJsonAsync("/api/loans", loan1);
        var loan2 = new CreateLoanRequest(book.Id, "Jane", "jane@test.com", DateTime.UtcNow.AddDays(14));
        var response = await _client.PostAsJsonAsync("/api/loans", loan2);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ReturnLoan_WhenActive_ReturnsOk()
    {
        var book = await CreateTestBookAsync();
        var loanReq = new CreateLoanRequest(book.Id, "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        var loanResp = await _client.PostAsJsonAsync("/api/loans", loanReq);
        var loan = await loanResp.Content.ReadFromJsonAsync<LoanResponse>();
        var response = await _client.PostAsync($"/api/loans/{loan!.Id}/return", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoanResponse>();
        result!.IsActive.Should().BeFalse();
        result.ReturnDate.Should().NotBeNull();
    }

    [Fact]
    public async Task ReturnLoan_AlreadyReturned_ReturnsConflict()
    {
        var book = await CreateTestBookAsync();
        var loanReq = new CreateLoanRequest(book.Id, "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        var loanResp = await _client.PostAsJsonAsync("/api/loans", loanReq);
        var loan = await loanResp.Content.ReadFromJsonAsync<LoanResponse>();
        await _client.PostAsync($"/api/loans/{loan!.Id}/return", null);
        var response = await _client.PostAsync($"/api/loans/{loan.Id}/return", null);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetLoans_FilterByActive_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/loans?active=true");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateLoan_InvalidEmail_ReturnsBadRequest()
    {
        var book = await CreateTestBookAsync();
        var request = new CreateLoanRequest(book.Id, "John", "not-email", DateTime.UtcNow.AddDays(14));
        var response = await _client.PostAsJsonAsync("/api/loans", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteBook_WithLoanHistory_ReturnsConflict()
    {
        var book = await CreateTestBookAsync();
        var loanReq = new CreateLoanRequest(book.Id, "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        var loanResp = await _client.PostAsJsonAsync("/api/loans", loanReq);
        var loan = await loanResp.Content.ReadFromJsonAsync<LoanResponse>();
        await _client.PostAsync($"/api/loans/{loan!.Id}/return", null);
        var response = await _client.DeleteAsync($"/api/books/{book.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
