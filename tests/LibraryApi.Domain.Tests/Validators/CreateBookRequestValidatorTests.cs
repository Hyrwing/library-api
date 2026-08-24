using FluentAssertions;
using FluentValidation.TestHelper;
using LibraryApi.Application.DTOs.Books;
using LibraryApi.Application.Validators;

namespace LibraryApi.Domain.Tests.Validators;

public class CreateBookRequestValidatorTests
{
    private readonly CreateBookRequestValidator _validator = new();

    [Fact]
    public void Valid_Request_Passes()
    {
        var request = new CreateBookRequest("Title", "Author", "9780743273565", 2004, "Fiction");
        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Title_Fails()
    {
        var request = new CreateBookRequest("", "Author", "9780743273565", 2004, "Fiction");
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Invalid_Isbn_Fails()
    {
        var request = new CreateBookRequest("Title", "Author", "ABC", 2004, "Fiction");
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Isbn);
    }

    [Fact]
    public void PublishedYear_TooOld_Fails()
    {
        var request = new CreateBookRequest("Title", "Author", "9780743273565", 1000, "Fiction");
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.PublishedYear);
    }
}
