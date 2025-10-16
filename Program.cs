using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

const string DataFile = "books.json";

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true
};

List<Book> books = Load();
bool dirty = false;

ShowWelcome();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== * BOOK MANAGER * ===");
    Console.WriteLine($"Items: {books.Count} | File: {DataFile} | Unsaved changes: {(dirty ? "YES" : "NO")}");
    Console.WriteLine("----------------------");
    Console.WriteLine("[1] View All");
    Console.WriteLine("[2] Add");
    Console.WriteLine("[3] Edit");
    Console.WriteLine("[4] Delete");
    Console.WriteLine("[5] Search");
    Console.WriteLine("[6] Sort");
    Console.WriteLine("[7] Export to CSV");
    Console.WriteLine("[8] Seed Sample Data");
    Console.WriteLine("[9] Clear Library");
    Console.WriteLine("[S] Save");
    Console.WriteLine("[Q] Quit");
    Console.Write("Choose an Option: ");
    var choice = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

    Console.WriteLine();

    switch (choice)
    {
        case "1": ListBooks(books); Pause(); break;
        case "2": AddBook(); break;
        case "3": EditBook(); break;
        case "4": DeleteBook(); break;
        case "5": SearchBooks(); break;
        case "6": SortBooks(); break;
        case "7": ExportCsv(); break;
        case "8": Seed(); break;
        case "9": ClearAll(); break;
        case "S": SaveIfDirty(); break;
        case "Q": SaveIfDirty(); return;
        default: Console.WriteLine("Unknown option. Please choose from the menu."); break;
    }
}

void ShowWelcome()
{
    Console.WriteLine("=== * BOOK MANAGER * ===");
    Console.WriteLine();
    Console.WriteLine("* TIP: You can open books.json in any editor to inspect saved data. *");
}

List<Book> Load()
{
    if (!File.Exists(DataFile)) return new List<Book>();
    try
    {
        var json = File.ReadAllText(DataFile);
        return JsonSerializer.Deserialize<List<Book>>(json, jsonOptions) ?? new List<Book>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[!] Failed to read {DataFile}. Starting empty. Details: {ex.Message}");
        return new List<Book>();
    }
}

void Save()
{
    try
    {
        File.WriteAllText(DataFile, JsonSerializer.Serialize(books, jsonOptions));
        dirty = false;
        Console.WriteLine($"Saved {books.Count} book(s) to {DataFile}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[!] Save failed: {ex.Message}");
    }
}

void SaveIfDirty()
{
    if (dirty && Confirm("[!] You have unsaved changes. Save? [Y/N] ")) Save();
    Console.WriteLine("Goodbye!");
}

void ListBooks(List<Book> list)
{
    if (!list.Any()) { Console.WriteLine("(No books yet...)"); return; }

    int wId = Math.Max(2, list.Max(b => b.Id.ToString().Length));
    int wTitle = Math.Max(5, Math.Max("Title".Length, list.Max(b => b.Title.Length)));
    int wAuthor = Math.Max(6, Math.Max("Author".Length, list.Max(b => b.Author.Length)));
    int wYear = 4;
    int wGenre = Math.Max(5, Math.Max("Genre".Length, list.Max(b => b.Genre?.Length ?? 0)));

    string sep = new string('-', wId + wTitle + wAuthor + wYear + wGenre + 13);
    Console.WriteLine(sep);
    Console.WriteLine($"| {"ID".PadRight(wId)} | {"Title".PadRight(wTitle)} | {"Author".PadRight(wAuthor)} | {"Year".PadRight(wYear)} | {"Genre".PadRight(wGenre)} |");
    Console.WriteLine(sep);

    foreach (var b in list)
    {
        Console.WriteLine($"| {b.Id.ToString().PadRight(wId)} | {b.Title.PadRight(wTitle)} | {b.Author.PadRight(wAuthor)} | {b.Year.ToString().PadRight(wYear)} | {(b.Genre ?? "").PadRight(wGenre)} |");
    }

    Console.WriteLine(sep);
}

void AddBook()
{
    Console.WriteLine("Add a New Book\n===");

    var book = new Book
    {
        Id = NextId(),
        Title = PromptNonEmpty("Title: "),
        Author = PromptNonEmpty("Author: "),
        Year = PromptInt($"Year (0-{DateTime.Now.Year}): ", 0, DateTime.Now.Year),
        Genre = PromptOptional("Genre (Optional): ")
    };

    books.Add(book);
    dirty = true;
    Console.WriteLine("Book added.");
}

void EditBook()
{
    if (!books.Any()) { Console.WriteLine("Nothing to edit."); return; }

    ListBooks(books);
    int id = PromptInt("Enter ID of the book to edit: ", 1, int.MaxValue);
    var book = books.FirstOrDefault(b => b.Id == id);
    if (book == null) { Console.WriteLine("Not found."); return; }

    Console.WriteLine("Leave blank to keep current value.");
    book.Title = PromptDefault($"Title [{book.Title}]: ", book.Title);
    book.Author = PromptDefault($"Author [{book.Author}]: ", book.Author);
    book.Year = PromptIntDefault($"Year [{book.Year}]: ", 0, DateTime.Now.Year, book.Year);
    string genreInput = PromptDefault($"Genre [{book.Genre ?? "none"}]: ", book.Genre ?? "");
    book.Genre = string.IsNullOrWhiteSpace(genreInput) ? null : genreInput;

    dirty = true;
    Console.WriteLine("Updated.");
}

void DeleteBook()
{
    if (!books.Any()) { Console.WriteLine("Nothing to delete."); return; }

    ListBooks(books);
    int id = PromptInt("Enter ID of the book to delete: ", 1, int.MaxValue);
    var book = books.FirstOrDefault(b => b.Id == id);
    if (book == null) { Console.WriteLine("Not found."); return; }

    if (Confirm($"Delete \"{book.Title}\" by {book.Author}? [y/N] "))
    {
        books.Remove(book);
        dirty = true;
        Console.WriteLine("Deleted.");
    }
    else Console.WriteLine("Cancelled.");
}

void SearchBooks()
{
    if (!books.Any()) { Console.WriteLine("No data to search."); return; }

    string term = PromptNonEmpty("Search term (Title/Author/Genre): ");
    var hits = books.Where(b =>
        b.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
        b.Author.Contains(term, StringComparison.OrdinalIgnoreCase) ||
        (b.Genre?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
    ).ToList();

    Console.WriteLine($"Found {hits.Count} result(s).");
    ListBooks(hits);
    Pause();
}

void SortBooks()
{
    if (!books.Any()) { Console.WriteLine("Nothing to sort."); return; }

    Console.WriteLine("Sort by: T)itle  A)uthor  Y)ear  G)enre");
    string by = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
    bool desc = (Console.ReadLine() ?? "").Trim().ToUpperInvariant() == "Y";

    books = (by switch
    {
        "A" => desc ? books.OrderByDescending(b => b.Author) : books.OrderBy(b => b.Author),
        "Y" => desc ? books.OrderByDescending(b => b.Year) : books.OrderBy(b => b.Year),
        "G" => desc ? books.OrderByDescending(b => b.Genre ?? "") : books.OrderBy(b => b.Genre ?? ""),
        _ => desc ? books.OrderByDescending(b => b.Title) : books.OrderBy(b => b.Title)
    }).ToList();

    dirty = true;
    Console.WriteLine("Sorted.");
    ListBooks(books);
    Pause();
}

void ExportCsv()
{
    if (!books.Any()) { Console.WriteLine("[!] No data to export."); return; }

    const string csvFile = "books.csv";
    try
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,Title,Author,Year,Genre");

        foreach (var b in books)
        {
            sb.AppendLine(string.Join(",",
                CsvEscape(b.Id.ToString()),
                CsvEscape(b.Title),
                CsvEscape(b.Author),
                CsvEscape(b.Year.ToString()),
                CsvEscape(b.Genre ?? "")
            ));
        }

        File.WriteAllText(csvFile, sb.ToString(), Encoding.UTF8);
        Console.WriteLine($"Exported to {csvFile}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[!] Export failed: {ex.Message}");
    }

    static string CsvEscape(string s)
    {
        if (s.Contains('"')) s = s.Replace("\"", "\"\"");
        if (s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r')) s = $"\"{s}\"";
        return s;
    }
}

void Seed()
{
    if (books.Any() && !Confirm("This will add sample books to your existing list. Continue? [Y/N] ")) { Console.WriteLine("Cancelled."); return; }

    var sample = new List<Book>
    {
        new() { Id = NextId(), Title = "Gödel, Escher, Bach", Author = "Douglas Hofstadter", Year = 1979, Genre = "Philosophy" },
        new() { Id = NextId() + 1, Title = "Simulacra and Simulation", Author = "Jean Baudrillard", Year = 1981, Genre = "Philosophy" },
        new() { Id = NextId() + 2, Title = "Zen and the Art of Motorcycle Maintenance", Author = "Robert M. Pirsig", Year = 1974, Genre = "Biography" }
    };

    books.AddRange(sample);
    ReassignIds();
    dirty = true;
    Console.WriteLine("Sample data added.");
}

void ClearAll()
{
    if (!books.Any()) { Console.WriteLine("Already empty."); return; }
    if (!Confirm("[!] This will remove ALL books. Are you sure? [Y/N] ")) { Console.WriteLine("Cancelled."); return; }

    books.Clear();
    dirty = true;
    Console.WriteLine("Cleared.");
}

int NextId() => books.Any() ? books.Max(b => b.Id) + 1 : 1;

void ReassignIds()
{
    int id = 1;
    foreach (var b in books.OrderBy(b => b.Id)) b.Id = id++;
}

string PromptNonEmpty(string label)
{
    while (true)
    {
        Console.Write(label);
        var s = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
        Console.WriteLine("Please enter a value.");
    }
}

string? PromptOptional(string label)
{
    Console.Write(label);
    var input = Console.ReadLine();
    return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
}

string PromptDefault(string label, string current)
{
    Console.Write(label);
    var s = Console.ReadLine();
    return string.IsNullOrWhiteSpace(s) ? current : s.Trim();
}

int PromptInt(string label, int min, int max)
{
    while (true)
    {
        Console.Write(label);
        if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max) return value;
        Console.WriteLine($"Enter a number between {min} and {max}.");
    }
}

int PromptIntDefault(string label, int min, int max, int current)
{
    while (true)
    {
        Console.Write(label);
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return current;
        if (int.TryParse(input, out int value) && value >= min && value <= max) return value;
        Console.WriteLine($"Enter a number between {min} and {max}, or press [ENTER] to keep {current}.");
    }
}

bool Confirm(string label)
{
    Console.Write(label);
    var s = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
    return s is "Y" or "YES";
}

void Pause()
{
    Console.WriteLine();
    Console.Write("Press [ENTER] to continue...");
    Console.ReadLine();
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Genre { get; set; }
}
