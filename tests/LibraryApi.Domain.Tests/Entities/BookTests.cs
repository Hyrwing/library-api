using FluentAssertions;
using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Tests.Entities;

public class BookTests
{
    [Fact]
    public void CanBeLoan_WhenNoActiveLoans_ReturnsTrue()
    {
        var book = new Book { Title = "Test" };
        book.CanBeLoan().Should().BeTrue();
    }

    [Fact]
    public void CanBeLoan_WhenActiveLoanExists_ReturnsFalse()
    {
        var book = new Book { Title = "Test" };
        book.Loans.Add(new Loan(book.Id, "John", "j@t.com", DateTime.UtcNow.AddDays(14)));
        book.CanBeLoan().Should().BeFalse();
    }

    [Fact]
    public void CanBeLoan_WhenAllLoansReturned_ReturnsTrue()
    {
        var book = new Book { Title = "Test" };
        var loan = new Loan(book.Id, "John", "j@t.com", DateTime.UtcNow.AddDays(14));
        loan.Return();
        book.Loans.Add(loan);
        book.CanBeLoan().Should().BeTrue();
    }

    [Fact]
    public void HasActiveLoan_WhenNoLoans_ReturnsFalse()
    {
        var book = new Book();
        book.HasActiveLoan.Should().BeFalse();
    }
}
