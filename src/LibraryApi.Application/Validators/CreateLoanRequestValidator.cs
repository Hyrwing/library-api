using FluentValidation;
using LibraryApi.Application.DTOs.Loans;

namespace LibraryApi.Application.Validators;

public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.BorrowerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BorrowerEmail).NotEmpty().MaximumLength(150).EmailAddress();
        RuleFor(x => x.DueDate).GreaterThan(DateTime.UtcNow)
            .WithMessage("Due date must be in the future.");
    }
}
