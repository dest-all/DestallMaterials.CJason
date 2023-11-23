namespace GenTests;

public static class TestingExtensions
{
    public static string ToStringUntilTerminator(this Span<char> chars)
        => new(chars.ToArray().TakeWhile(c => c != '\0').ToArray());
}
