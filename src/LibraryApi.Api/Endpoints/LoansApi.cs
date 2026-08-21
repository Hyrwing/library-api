using LibraryApi.Api.Infrastructure;
using LibraryApi.Application.DTOs.Loans;
using LibraryApi.Application.Services;

namespace LibraryApi.Api.Endpoints;

public static class LoansApi
{
    public static RouteGroupBuilder MapLoans(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/loans").WithTags("Loans");
        group.MapGet("/", ListLoans);
        group.MapGet("/{id:guid}", GetLoan);
        group.MapPost("/", CreateLoan).AddEndpointFilter<ValidationFilter<CreateLoanRequest>>();
        group.MapPost("/{id:guid}/return", ReturnLoan);
        return group;
    }

    private static async Task<IResult> ListLoans(
        string? search, Guid? bookId, bool? active,
        int? page, int? pageSize,
        LoanService loanService, CancellationToken ct)
    {
        var result = await loanService.ListAsync(search, bookId, active, page ?? 1, pageSize ?? 10, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetLoan(Guid id, LoanService loanService, CancellationToken ct)
    {
        var result = await loanService.GetByIdAsync(id, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> CreateLoan(CreateLoanRequest request, LoanService loanService, CancellationToken ct)
    {
        var result = await loanService.CreateAsync(request, ct);
        return TypedResults.Created($"/api/loans/{result.Id}", result);
    }

    private static async Task<IResult> ReturnLoan(Guid id, LoanService loanService, CancellationToken ct)
    {
        var result = await loanService.ReturnAsync(id, ct);
        return TypedResults.Ok(result);
    }
}
