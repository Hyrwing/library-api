using FluentAssertions;
using FluentValidation.TestHelper;
using LibraryApi.Application.DTOs.Loans;
using LibraryApi.Application.Validators;

namespace LibraryApi.Domain.Tests.Validators;

public class CreateLoanRequestValidatorTests
{
    private readonly CreateLoanRequestValidator _validator = new();

    [Fact]
    public void Valid_Request_Passes()
    {
        var request = new CreateLoanRequest(Guid.NewGuid(), "John", "john@email.com", DateTime.UtcNow.AddDays(14));
        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_BorrowerName_Fails()
    {
        var request = new CreateLoanRequest(Guid.NewGuid(), "", "john@email.com", DateTime.UtcNow.AddDays(14));
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.BorrowerName);
    }

    [Fact]
    public void Invalid_Email_Fails()
    {
        var request = new CreateLoanRequest(Guid.NewGuid(), "John", "not-email", DateTime.UtcNow.AddDays(14));
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.BorrowerEmail);
    }

    [Fact]
    public void PastDueDate_Fails()
    {
        var request = new CreateLoanRequest(Guid.NewGuid(), "John", "john@email.com", DateTime.UtcNow.AddDays(-1));
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.DueDate);
    }
}
