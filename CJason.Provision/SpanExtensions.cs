using System.Runtime.InteropServices;

namespace CJason.Provision;

public static class SpanExtensions
{
    public static char NextNonEmptySymbol(this Span<char> chars, out int passed)
    {
        var l = chars.Length;
        passed = 0;
        if (l == 0)
        {
            
            return default;
        }
        char c;
        do 
        {
            c = chars[passed];
        }
        while ((c == '\t' || c == '\n' || c == '\r' || c == ' ' || c == '\0') && passed < l);
        return c;
    }

    public static int NextAt(this Span<char> chars, char soughtSymbol)
    {
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == soughtSymbol)
            {
                return i;
            }
        }
        return -1;
    }
    
    public static Span<T> EnsureSpan<T>(this IEnumerable<T> source)
    {
        if (source is T[] span)
        {
            return span;
        }
        if (source is List<T> list)
        {
            return CollectionsMarshal.AsSpan(list);
        }
        var array = source.ToArray();
        return array;
    }

    public static int CopiedTo<T>(this Span<T> source, Span<T> target)
    {
        source.CopyTo(target);
        return source.Length;
    }

    public static int CopiedTo<T>(this ReadOnlySpan<T> source, Span<T> target)
    {
        source.CopyTo(target);
        return source.Length;
    }
}
