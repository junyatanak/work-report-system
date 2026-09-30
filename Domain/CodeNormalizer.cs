namespace DailyWorkReport.Domain;

public static class CodeNormalizer
{
    /// <summary>
    /// Normalizes code-like values (e.g. order numbers, product codes)
    /// by trimming whitespace and converting to uppercase.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Normalize(string? value) => (value ?? string.Empty).Trim().ToUpperInvariant();
}