using System.Text.RegularExpressions;

public static class ISBNFormatChecker
{
    private static readonly Regex Isbn10Regex = new Regex(@"^\d{9}[\dXx]$", RegexOptions.Compiled);
    private static readonly Regex Isbn13Regex = new Regex(@"^\d{13}$", RegexOptions.Compiled);

    public static bool IsValidIsbn(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var cleaned = NormalizeIsbn(input);
        if (Isbn10Regex.IsMatch(cleaned))
            return IsValidIsbn10(cleaned);
        if (Isbn13Regex.IsMatch(cleaned))
            return IsValidIsbn13(cleaned);
        return false;
    }

    public static bool IsValidIsbn10(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var cleaned = NormalizeIsbn(input);
        if (!Isbn10Regex.IsMatch(cleaned)) return false;
        // Checksum: sum(i * digit_i) for i=1..10 where X=10 must be divisible by 11
        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            int val = (i == 9 && cleaned[i] == 'X') ? 10 : (cleaned[i] - '0');
            sum += (i + 1) * val;
        }
        return sum % 11 == 0;
    }

    public static bool IsValidIsbn13(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var cleaned = NormalizeIsbn(input);
        if (!Isbn13Regex.IsMatch(cleaned)) return false;
        // Checksum: sum of digits with weights 1 and 3 alternating; checksum digit makes sum % 10 == 0
        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            int digit = cleaned[i] - '0';
            sum += digit * ((i % 2 == 0) ? 1 : 3);
        }
        int check = (10 - (sum % 10)) % 10;
        int last = cleaned[12] - '0';
        return check == last;
    }

    /// <summary>
    /// Removes hyphens, spaces, and uppercases X.
    /// </summary>
    public static string NormalizeIsbn(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var cleaned = Regex.Replace(input, "[^0-9Xx]", string.Empty);
        return cleaned.ToUpperInvariant();
    }
}
