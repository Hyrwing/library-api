using FluentAssertions;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;

namespace LibraryApi.Domain.Tests.Entities;

public class LoanTests
{
    [Fact]
    public void Return_WhenActive_SetsReturnDate()
    {
        var loan = new Loan(Guid.NewGuid(), "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        loan.Return();
        loan.ReturnDate.Should().NotBeNull();
        loan.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Return_WhenAlreadyReturned_ThrowsLoanAlreadyReturnedException()
    {
        var loan = new Loan(Guid.NewGuid(), "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        loan.Return();
        var act = () => loan.Return();
        act.Should().Throw<LoanAlreadyReturnedException>();
    }

    [Fact]
    public void Constructor_WhenDueDateBeforeLoanDate_ThrowsDomainException()
    {
        var act = () => new Loan(Guid.NewGuid(), "John", "john@test.com", DateTime.UtcNow.AddDays(-1));
        act.Should().Throw<DomainException>().WithMessage("*after*");
    }

    [Fact]
    public void Constructor_WhenDueDateAfterLoanDate_DoesNotThrow()
    {
        var act = () => new Loan(Guid.NewGuid(), "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        act.Should().NotThrow();
    }

    [Fact]
    public void IsActive_WhenNewLoan_ReturnsTrue()
    {
        var loan = new Loan(Guid.NewGuid(), "John", "john@test.com", DateTime.UtcNow.AddDays(14));
        loan.IsActive.Should().BeTrue();
    }
}
