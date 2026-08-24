namespace LibraryApi.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public int PublishedYear { get; set; }
    public string Genre { get; set; } = string.Empty;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public bool HasActiveLoan => Loans.Any(l => l.ReturnDate == null);

    public bool CanBeLoan()
    {
        return !HasActiveLoan;
    }
}
