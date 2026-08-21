using FluentValidation;
using LibraryApi.Application.DTOs.Books;

namespace LibraryApi.Application.Validators;

public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Isbn).NotEmpty().MaximumLength(13)
            .Matches(@"^\d{10}(\d{3})?$").WithMessage("ISBN must be 10 or 13 digits.");
        RuleFor(x => x.PublishedYear)
            .InclusiveBetween(1450, DateTime.UtcNow.Year)
            .WithMessage($"Published year must be between 1450 and {DateTime.UtcNow.Year}.");
        RuleFor(x => x.Genre).NotEmpty().MaximumLength(50);
    }
}
