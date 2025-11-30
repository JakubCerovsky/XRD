using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages a database of books with their ISBNs and corresponding GameObjects.
/// Used to map ISBN input to book locations in the scene.
/// </summary>
public class BookDatabase : MonoBehaviour
{
    [System.Serializable]
    public class BookEntry
    {
        [Tooltip("The ISBN number of the book")]
        public string isbn;

        [Tooltip("The GameObject representing the book location in the scene")]
        public GameObject bookObject;

        [Tooltip("Optional: Book title for display purposes")]
        public string title;

        public BookEntry(string isbn, GameObject bookObject, string title = "")
        {
            this.isbn = isbn;
            this.bookObject = bookObject;
            this.title = title;
        }
    }

    [Header("Book Database")]
    [Tooltip("List of books with their ISBNs and corresponding GameObjects")]
    [SerializeField]
    private List<BookEntry> books = new List<BookEntry>();

    [Header("Auto-Discovery")]
    [Tooltip("Automatically find all GameObjects with the 'Book' tag and add them to the database at startup")]
    [SerializeField]
    private bool autoDiscoverBooks = true;

    [Tooltip("Tag to use when auto-discovering books")]
    [SerializeField]
    private string bookTag = "Book";

    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField]
    private bool showDebugLogs = true;

    void Start()
    {
        if (autoDiscoverBooks)
        {
            AutoDiscoverBooks();
        }

        LogBookDatabase();
    }

    /// <summary>
    /// Automatically discovers all GameObjects with the specified tag and adds them to the database.
    /// </summary>
    void AutoDiscoverBooks()
    {
        Log("Auto-discovering books in scene...");

        try
        {
            GameObject[] foundBooks = GameObject.FindGameObjectsWithTag(bookTag);
            Log($"Found {foundBooks.Length} GameObject(s) with tag '{bookTag}'");

            foreach (GameObject bookObj in foundBooks)
            {
                // Check if this book is already in the database
                bool alreadyExists = books.Any(b => b.bookObject == bookObj);
                
                if (!alreadyExists)
                {
                    // Use the GameObject's name as the ISBN if not manually configured
                    string isbn = bookObj.name;
                    books.Add(new BookEntry(isbn, bookObj, bookObj.name));
                    Log($"  - Added book: '{bookObj.name}' (ISBN: {isbn}) from scene '{bookObj.scene.name}'");
                }
                else
                {
                    Log($"  - Book '{bookObj.name}' already in database, skipping");
                }
            }
        }
        catch (UnityException e)
        {
            LogError($"Tag '{bookTag}' is not defined! Error: {e.Message}");
            LogError("Go to Edit → Project Settings → Tags and Layers to add the 'Book' tag.");
        }
    }

    /// <summary>
    /// Finds a book GameObject by ISBN.
    /// </summary>
    /// <param name="isbn">The ISBN to search for</param>
    /// <returns>The GameObject if found, null otherwise</returns>
    public GameObject FindBookByISBN(string isbn)
    {
        if (string.IsNullOrEmpty(isbn))
        {
            LogWarning("ISBN is null or empty!");
            return null;
        }

        // Try exact match first
        BookEntry exactMatch = books.FirstOrDefault(b => 
            b.isbn != null && b.isbn.Equals(isbn, StringComparison.OrdinalIgnoreCase));
        
        if (exactMatch != null && exactMatch.bookObject != null)
        {
            Log($"✓ Found book by ISBN '{isbn}': {exactMatch.bookObject.name} (Title: {exactMatch.title})");
            return exactMatch.bookObject;
        }

        // Try partial match (contains)
        BookEntry partialMatch = books.FirstOrDefault(b => 
            b.isbn != null && b.isbn.IndexOf(isbn, StringComparison.OrdinalIgnoreCase) >= 0);
        
        if (partialMatch != null && partialMatch.bookObject != null)
        {
            Log($"✓ Found book by partial ISBN match '{isbn}' → '{partialMatch.isbn}': {partialMatch.bookObject.name}");
            return partialMatch.bookObject;
        }

        // Try matching by title
        BookEntry titleMatch = books.FirstOrDefault(b => 
            !string.IsNullOrEmpty(b.title) && 
            b.title.IndexOf(isbn, StringComparison.OrdinalIgnoreCase) >= 0);
        
        if (titleMatch != null && titleMatch.bookObject != null)
        {
            Log($"✓ Found book by title match '{isbn}' → '{titleMatch.title}': {titleMatch.bookObject.name}");
            return titleMatch.bookObject;
        }

        LogWarning($"No book found with ISBN '{isbn}' in database ({books.Count} books registered)");
        return null;
    }

    /// <summary>
    /// Adds a book to the database programmatically.
    /// </summary>
    /// <param name="isbn">The ISBN of the book</param>
    /// <param name="bookObject">The GameObject representing the book</param>
    /// <param name="title">Optional title</param>
    public void AddBook(string isbn, GameObject bookObject, string title = "")
    {
        if (string.IsNullOrEmpty(isbn))
        {
            LogWarning("Cannot add book: ISBN is null or empty");
            return;
        }

        if (bookObject == null)
        {
            LogWarning($"Cannot add book with ISBN '{isbn}': GameObject is null");
            return;
        }

        // Check if already exists
        BookEntry existing = books.FirstOrDefault(b => b.isbn == isbn);
        if (existing != null)
        {
            LogWarning($"Book with ISBN '{isbn}' already exists in database. Updating...");
            existing.bookObject = bookObject;
            existing.title = title;
        }
        else
        {
            books.Add(new BookEntry(isbn, bookObject, title));
            Log($"Added book: ISBN '{isbn}', Object '{bookObject.name}', Title '{title}'");
        }
    }

    /// <summary>
    /// Removes a book from the database by ISBN.
    /// </summary>
    /// <param name="isbn">The ISBN of the book to remove</param>
    public void RemoveBook(string isbn)
    {
        int removed = books.RemoveAll(b => b.isbn == isbn);
        if (removed > 0)
        {
            Log($"Removed {removed} book(s) with ISBN '{isbn}'");
        }
        else
        {
            LogWarning($"No book found with ISBN '{isbn}' to remove");
        }
    }

    /// <summary>
    /// Gets all books in the database.
    /// </summary>
    public List<BookEntry> GetAllBooks()
    {
        return new List<BookEntry>(books);
    }

    /// <summary>
    /// Gets the total number of books in the database.
    /// </summary>
    public int GetBookCount()
    {
        return books.Count;
    }

    /// <summary>
    /// Logs the entire book database for debugging.
    /// </summary>
    void LogBookDatabase()
    {
        Log($"=== Book Database ===");
        Log($"Total books: {books.Count}");
        
        if (books.Count > 0)
        {
            foreach (var book in books)
            {
                string objName = book.bookObject != null ? book.bookObject.name : "NULL";
                string objScene = book.bookObject != null ? book.bookObject.scene.name : "N/A";
                Log($"  - ISBN: '{book.isbn}' | Object: '{objName}' | Title: '{book.title}' | Scene: '{objScene}'");
            }
        }
        else
        {
            LogWarning("Database is empty! Make sure books are added manually or auto-discovery is enabled.");
        }
        
        Log($"====================");
    }

    void Log(string message)
    {
        if (showDebugLogs)
            Debug.Log($"[BookDatabase] {message}");
    }

    void LogWarning(string message)
    {
        Debug.LogWarning($"[BookDatabase] {message}");
    }

    void LogError(string message)
    {
        Debug.LogError($"[BookDatabase] {message}");
    }
}
