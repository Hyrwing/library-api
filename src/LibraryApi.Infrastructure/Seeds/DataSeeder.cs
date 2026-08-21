using LibraryApi.Domain.Entities;
using LibraryApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibraryApi.Infrastructure.Seeds;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LibraryDbContext>>();

        if (await db.Books.AnyAsync()) { logger.LogInformation("Database already seeded."); return; }

        db.Books.AddRange(GetBooks());
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} books.", 100);
    }

    private static List<Book> GetBooks() =>
    [
        // Classic Literature (20)
        new() { Title = "To Kill a Mockingbird", Author = "Harper Lee", Isbn = "9780061120084", PublishedYear = 1960, Genre = "Classic" },
        new() { Title = "1984", Author = "George Orwell", Isbn = "9780451524935", PublishedYear = 1949, Genre = "Classic" },
        new() { Title = "Pride and Prejudice", Author = "Jane Austen", Isbn = "9780141439518", PublishedYear = 1813, Genre = "Classic" },
        new() { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Isbn = "9780743273565", PublishedYear = 1925, Genre = "Classic" },
        new() { Title = "One Hundred Years of Solitude", Author = "Gabriel García Márquez", Isbn = "9780060883287", PublishedYear = 1967, Genre = "Classic" },
        new() { Title = "Moby Dick", Author = "Herman Melville", Isbn = "9780142437247", PublishedYear = 1851, Genre = "Classic" },
        new() { Title = "War and Peace", Author = "Leo Tolstoy", Isbn = "9780199232765", PublishedYear = 1869, Genre = "Classic" },
        new() { Title = "Crime and Punishment", Author = "Fyodor Dostoevsky", Isbn = "9780486415871", PublishedYear = 1866, Genre = "Classic" },
        new() { Title = "The Catcher in the Rye", Author = "J.D. Salinger", Isbn = "9780316769488", PublishedYear = 1951, Genre = "Classic" },
        new() { Title = "Brave New World", Author = "Aldous Huxley", Isbn = "9780060850524", PublishedYear = 1932, Genre = "Classic" },
        new() { Title = "Jane Eyre", Author = "Charlotte Brontë", Isbn = "9780141441146", PublishedYear = 1847, Genre = "Classic" },
        new() { Title = "Wuthering Heights", Author = "Emily Brontë", Isbn = "9780141439556", PublishedYear = 1847, Genre = "Classic" },
        new() { Title = "Great Expectations", Author = "Charles Dickens", Isbn = "9780141439563", PublishedYear = 1861, Genre = "Classic" },
        new() { Title = "The Brothers Karamazov", Author = "Fyodor Dostoevsky", Isbn = "9780374528379", PublishedYear = 1880, Genre = "Classic" },
        new() { Title = "Anna Karenina", Author = "Leo Tolstoy", Isbn = "9780143035008", PublishedYear = 1877, Genre = "Classic" },
        new() { Title = "Don Quixote", Author = "Miguel de Cervantes", Isbn = "9780060934347", PublishedYear = 1605, Genre = "Classic" },
        new() { Title = "The Odyssey", Author = "Homer", Isbn = "9780140268867", PublishedYear = 1614, Genre = "Classic" },
        new() { Title = "Frankenstein", Author = "Mary Shelley", Isbn = "9780141439471", PublishedYear = 1818, Genre = "Classic" },
        new() { Title = "Dracula", Author = "Bram Stoker", Isbn = "9780141439846", PublishedYear = 1897, Genre = "Classic" },
        new() { Title = "The Count of Monte Cristo", Author = "Alexandre Dumas", Isbn = "9780140449266", PublishedYear = 1844, Genre = "Classic" },

        // Science Fiction (20)
        new() { Title = "Dune", Author = "Frank Herbert", Isbn = "9780441013593", PublishedYear = 1965, Genre = "Science Fiction" },
        new() { Title = "Foundation", Author = "Isaac Asimov", Isbn = "9780553293357", PublishedYear = 1951, Genre = "Science Fiction" },
        new() { Title = "Neuromancer", Author = "William Gibson", Isbn = "9780441569595", PublishedYear = 1984, Genre = "Science Fiction" },
        new() { Title = "The Left Hand of Darkness", Author = "Ursula K. Le Guin", Isbn = "9780441478125", PublishedYear = 1969, Genre = "Science Fiction" },
        new() { Title = "Ender's Game", Author = "Orson Scott Card", Isbn = "9780812550702", PublishedYear = 1985, Genre = "Science Fiction" },
        new() { Title = "The Hitchhiker's Guide to the Galaxy", Author = "Douglas Adams", Isbn = "9780345391803", PublishedYear = 1979, Genre = "Science Fiction" },
        new() { Title = "Fahrenheit 451", Author = "Ray Bradbury", Isbn = "9781451673319", PublishedYear = 1953, Genre = "Science Fiction" },
        new() { Title = "Snow Crash", Author = "Neal Stephenson", Isbn = "9780553380958", PublishedYear = 1992, Genre = "Science Fiction" },
        new() { Title = "The Martian", Author = "Andy Weir", Isbn = "9780553418026", PublishedYear = 2011, Genre = "Science Fiction" },
        new() { Title = "Hyperion", Author = "Dan Simmons", Isbn = "9780553283686", PublishedYear = 1989, Genre = "Science Fiction" },
        new() { Title = "2001: A Space Odyssey", Author = "Arthur C. Clarke", Isbn = "9780451457998", PublishedYear = 1968, Genre = "Science Fiction" },
        new() { Title = "Slaughterhouse-Five", Author = "Kurt Vonnegut", Isbn = "9780812988529", PublishedYear = 1969, Genre = "Science Fiction" },
        new() { Title = "The War of the Worlds", Author = "H.G. Wells", Isbn = "9780141441030", PublishedYear = 1898, Genre = "Science Fiction" },
        new() { Title = "Solaris", Author = "Stanislaw Lem", Isbn = "9780156027601", PublishedYear = 1961, Genre = "Science Fiction" },
        new() { Title = "Do Androids Dream of Electric Sheep?", Author = "Philip K. Dick", Isbn = "9780345404473", PublishedYear = 1968, Genre = "Science Fiction" },
        new() { Title = "Contact", Author = "Carl Sagan", Isbn = "9781501197987", PublishedYear = 1985, Genre = "Science Fiction" },
        new() { Title = "The Time Machine", Author = "H.G. Wells", Isbn = "9780141439976", PublishedYear = 1895, Genre = "Science Fiction" },
        new() { Title = "Ringworld", Author = "Larry Niven", Isbn = "9780345333926", PublishedYear = 1970, Genre = "Science Fiction" },
        new() { Title = "Rendezvous with Rama", Author = "Arthur C. Clarke", Isbn = "9780553287899", PublishedYear = 1973, Genre = "Science Fiction" },
        new() { Title = "The Dispossessed", Author = "Ursula K. Le Guin", Isbn = "9780061054884", PublishedYear = 1974, Genre = "Science Fiction" },

        // Mystery (20)
        new() { Title = "The Girl with the Dragon Tattoo", Author = "Stieg Larsson", Isbn = "9780307454546", PublishedYear = 2005, Genre = "Mystery" },
        new() { Title = "Gone Girl", Author = "Gillian Flynn", Isbn = "9780307588371", PublishedYear = 2012, Genre = "Mystery" },
        new() { Title = "The Da Vinci Code", Author = "Dan Brown", Isbn = "9780307474278", PublishedYear = 2003, Genre = "Mystery" },
        new() { Title = "And Then There Were None", Author = "Agatha Christie", Isbn = "9780062073488", PublishedYear = 1939, Genre = "Mystery" },
        new() { Title = "The Silence of the Lambs", Author = "Thomas Harris", Isbn = "9780312924584", PublishedYear = 1988, Genre = "Mystery" },
        new() { Title = "In the Woods", Author = "Tana French", Isbn = "9780143113492", PublishedYear = 2007, Genre = "Mystery" },
        new() { Title = "Big Little Lies", Author = "Liane Moriarty", Isbn = "9780399587191", PublishedYear = 2014, Genre = "Mystery" },
        new() { Title = "The Hound of the Baskervilles", Author = "Arthur Conan Doyle", Isbn = "9780141199177", PublishedYear = 1902, Genre = "Mystery" },
        new() { Title = "Murder on the Orient Express", Author = "Agatha Christie", Isbn = "9780062693662", PublishedYear = 1934, Genre = "Mystery" },
        new() { Title = "The Maltese Falcon", Author = "Dashiell Hammett", Isbn = "9780679722649", PublishedYear = 1930, Genre = "Mystery" },
        new() { Title = "Rebecca", Author = "Daphne du Maurier", Isbn = "9780380730407", PublishedYear = 1938, Genre = "Mystery" },
        new() { Title = "The Name of the Rose", Author = "Umberto Eco", Isbn = "9780544176560", PublishedYear = 1980, Genre = "Mystery" },
        new() { Title = "Sharp Objects", Author = "Gillian Flynn", Isbn = "9780307341556", PublishedYear = 2006, Genre = "Mystery" },
        new() { Title = "The Woman in the Window", Author = "A.J. Finn", Isbn = "9780062678416", PublishedYear = 2018, Genre = "Mystery" },
        new() { Title = "The Girl on the Train", Author = "Paula Hawkins", Isbn = "9781594634024", PublishedYear = 2015, Genre = "Mystery" },
        new() { Title = "The Secret History", Author = "Donna Tartt", Isbn = "9781400031702", PublishedYear = 1992, Genre = "Mystery" },
        new() { Title = "The Big Sleep", Author = "Raymond Chandler", Isbn = "9780394758282", PublishedYear = 1939, Genre = "Mystery" },
        new() { Title = "Tinker Tailor Soldier Spy", Author = "John le Carré", Isbn = "9780143119784", PublishedYear = 1974, Genre = "Mystery" },
        new() { Title = "The No. 1 Ladies' Detective Agency", Author = "Alexander McCall Smith", Isbn = "9781400034772", PublishedYear = 1998, Genre = "Mystery" },
        new() { Title = "In Cold Blood", Author = "Truman Capote", Isbn = "9780679745587", PublishedYear = 1966, Genre = "Mystery" },

        // Fantasy (20)
        new() { Title = "The Hobbit", Author = "J.R.R. Tolkien", Isbn = "9780547928227", PublishedYear = 1937, Genre = "Fantasy" },
        new() { Title = "Harry Potter and the Sorcerer's Stone", Author = "J.K. Rowling", Isbn = "9780590353427", PublishedYear = 1997, Genre = "Fantasy" },
        new() { Title = "A Game of Thrones", Author = "George R.R. Martin", Isbn = "9780553573404", PublishedYear = 1996, Genre = "Fantasy" },
        new() { Title = "The Name of the Wind", Author = "Patrick Rothfuss", Isbn = "9780756404741", PublishedYear = 2007, Genre = "Fantasy" },
        new() { Title = "The Way of Kings", Author = "Brandon Sanderson", Isbn = "9780765365279", PublishedYear = 2010, Genre = "Fantasy" },
        new() { Title = "American Gods", Author = "Neil Gaiman", Isbn = "9780063081918", PublishedYear = 2001, Genre = "Fantasy" },
        new() { Title = "The Color of Magic", Author = "Terry Pratchett", Isbn = "9780062225672", PublishedYear = 1983, Genre = "Fantasy" },
        new() { Title = "A Wizard of Earthsea", Author = "Ursula K. Le Guin", Isbn = "9780547722023", PublishedYear = 1968, Genre = "Fantasy" },
        new() { Title = "The Lion, the Witch and the Wardrobe", Author = "C.S. Lewis", Isbn = "9780064404990", PublishedYear = 1950, Genre = "Fantasy" },
        new() { Title = "The Lies of Locke Lamora", Author = "Scott Lynch", Isbn = "9780553588941", PublishedYear = 2006, Genre = "Fantasy" },
        new() { Title = "Mistborn: The Final Empire", Author = "Brandon Sanderson", Isbn = "9780765311788", PublishedYear = 2006, Genre = "Fantasy" },
        new() { Title = "The Blade Itself", Author = "Joe Abercrombie", Isbn = "9780316387316", PublishedYear = 2006, Genre = "Fantasy" },
        new() { Title = "The Eye of the World", Author = "Robert Jordan", Isbn = "9780812511819", PublishedYear = 1990, Genre = "Fantasy" },
        new() { Title = "Good Omens", Author = "Terry Pratchett & Neil Gaiman", Isbn = "9780060853983", PublishedYear = 1990, Genre = "Fantasy" },
        new() { Title = "Assassin's Apprentice", Author = "Robin Hobb", Isbn = "9780553573398", PublishedYear = 1995, Genre = "Fantasy" },
        new() { Title = "The Fifth Season", Author = "N.K. Jemisin", Isbn = "9780316229296", PublishedYear = 2015, Genre = "Fantasy" },
        new() { Title = "The Once and Future King", Author = "T.H. White", Isbn = "9780441627400", PublishedYear = 1958, Genre = "Fantasy" },
        new() { Title = "Eragon", Author = "Christopher Paolini", Isbn = "9780375826696", PublishedYear = 2003, Genre = "Fantasy" },
        new() { Title = "The Princess Bride", Author = "William Goldman", Isbn = "9780156035217", PublishedYear = 1973, Genre = "Fantasy" },
        new() { Title = "Circe", Author = "Madeline Miller", Isbn = "9780316556347", PublishedYear = 2018, Genre = "Fantasy" },

        // Non-Fiction (20)
        new() { Title = "Sapiens", Author = "Yuval Noah Harari", Isbn = "9780062316097", PublishedYear = 2011, Genre = "Non-Fiction" },
        new() { Title = "Educated", Author = "Tara Westover", Isbn = "9780399590504", PublishedYear = 2018, Genre = "Non-Fiction" },
        new() { Title = "Thinking, Fast and Slow", Author = "Daniel Kahneman", Isbn = "9780374533557", PublishedYear = 2011, Genre = "Non-Fiction" },
        new() { Title = "The Power of Habit", Author = "Charles Duhigg", Isbn = "9780812981605", PublishedYear = 2012, Genre = "Non-Fiction" },
        new() { Title = "Atomic Habits", Author = "James Clear", Isbn = "9780735211292", PublishedYear = 2018, Genre = "Non-Fiction" },
        new() { Title = "A Brief History of Time", Author = "Stephen Hawking", Isbn = "9780553380163", PublishedYear = 1988, Genre = "Non-Fiction" },
        new() { Title = "The Immortal Life of Henrietta Lacks", Author = "Rebecca Skloot", Isbn = "9781400052189", PublishedYear = 2010, Genre = "Non-Fiction" },
        new() { Title = "Becoming", Author = "Michelle Obama", Isbn = "9781524763138", PublishedYear = 2018, Genre = "Non-Fiction" },
        new() { Title = "Quiet", Author = "Susan Cain", Isbn = "9780307352156", PublishedYear = 2012, Genre = "Non-Fiction" },
        new() { Title = "The Lean Startup", Author = "Eric Ries", Isbn = "9780307887894", PublishedYear = 2011, Genre = "Non-Fiction" },
        new() { Title = "Guns, Germs, and Steel", Author = "Jared Diamond", Isbn = "9780393317558", PublishedYear = 1997, Genre = "Non-Fiction" },
        new() { Title = "Outliers", Author = "Malcolm Gladwell", Isbn = "9780316017930", PublishedYear = 2008, Genre = "Non-Fiction" },
        new() { Title = "The Art of War", Author = "Sun Tzu", Isbn = "9781590302255", PublishedYear = 1910, Genre = "Non-Fiction" },
        new() { Title = "Freakonomics", Author = "Steven Levitt & Stephen Dubner", Isbn = "9780060731335", PublishedYear = 2005, Genre = "Non-Fiction" },
        new() { Title = "Clean Code", Author = "Robert C. Martin", Isbn = "9780132350884", PublishedYear = 2008, Genre = "Non-Fiction" },
        new() { Title = "The Pragmatic Programmer", Author = "Andrew Hunt & David Thomas", Isbn = "9780135957059", PublishedYear = 1999, Genre = "Non-Fiction" },
        new() { Title = "Deep Work", Author = "Cal Newport", Isbn = "9781455586691", PublishedYear = 2016, Genre = "Non-Fiction" },
        new() { Title = "Homo Deus", Author = "Yuval Noah Harari", Isbn = "9780062464316", PublishedYear = 2015, Genre = "Non-Fiction" },
        new() { Title = "Thinking in Systems", Author = "Donella H. Meadows", Isbn = "9781603580557", PublishedYear = 2008, Genre = "Non-Fiction" },
        new() { Title = "The Design of Everyday Things", Author = "Don Norman", Isbn = "9780465050659", PublishedYear = 1988, Genre = "Non-Fiction" },
    ];
}
