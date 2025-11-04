namespace HSEBank.Console.App;

public static class IdFormatter
{
    /// <summary>
    /// "ABCD…EF12" — удобный укороченный вид GUID.
    /// </summary>
    public static string Short(Guid id, int left = 4, int right = 4)
    {
        var s = id.ToString("N").ToUpperInvariant(); // без дефисов, 32 символа
        left = Math.Clamp(left, 1, 16);
        right = Math.Clamp(right, 1, 16);
        return $"{s[..left]}…{s[^right..]}";
    }

    /// <summary>
    /// Только хвост GUID (по умолчанию 6 символов), напр. "EF12AB".
    /// Удобно, если хочешь совсем компактно.
    /// </summary>
    public static string Tail(Guid id, int len = 6)
    {
        var s = id.ToString("N").ToUpperInvariant();
        len = Math.Clamp(len, 2, 16);
        return s[^len..];
    }
}