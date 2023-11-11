global using JsonPiece = System.ReadOnlySpan<char>;
using System.Globalization;
using System.Numerics;
using System.Text.Json;

namespace CJason.Provision;

public delegate JsonPiece ReadValue<T>(JsonPiece jsonPiece, out T parsedValue);

public static class JsonSerializationUtilities
{
    static void ThrowBadSymbolAt(char symbol, char expected, int position) =>
        throw new JsonException(
            $"Unexpected symbol encountered at {position}. Expected: {expected}. Actual: {symbol}."
        );

    public static int ReadPropertyIndex(
        Span<char> source,
        ReadOnlySpan<string> properties,
        int bufferSize,
        out int passed
    )
    {
        var l = properties.Length;

        Span<char> foundProperty = stackalloc char[bufferSize];

        int r = 0;
        var symbol = source.NextNonEmptySymbol(out r);
        if (symbol != '"')
        {
            ThrowBadSymbolAt(symbol, '"', r);
        }
        r++;
        source = source[r..];
        var nextQuoteAt = source.NextAt('"');
        source[..nextQuoteAt].CopyTo(foundProperty);

        foundProperty = foundProperty[..nextQuoteAt];

        var foundPropLength = nextQuoteAt - 1;

        passed = r + nextQuoteAt;

        for (int i = 0; i < properties.Length; i++)
        {
            var property = properties[i].AsSpan();
            var propLength = property.Length;
            if (propLength != foundPropLength)
            {
                continue;
            }
            bool isMatch = true;
            for (int j = 0; j < propLength; j++)
            {
                var c = property[j];
                if (c != foundProperty[j])
                {
                    isMatch = false;
                    break;
                }
            }
            if (isMatch)
            {
                return i;
            }
        }

        return -1;
    }

    public static JsonPiece RemovePropertyName(this JsonPiece jsonPiece, out JsonPiece propertyName)
    {
        var firstSymbol = jsonPiece[0];

        if (firstSymbol != '"')
        {
            throw new JsonException();
        }

        int i = 1;
        for (; i < jsonPiece.Length; i++)
        {
            var c = jsonPiece[i];
            if (c == '"')
            {
                break;
            }
        }

        propertyName = jsonPiece[1..i];

        return jsonPiece[(i + 1)..];
    }

    public static JsonPiece SkipToPropertyName(this JsonPiece jsonPiece)
    {
        jsonPiece = jsonPiece.SkipInsignificantSymbolsLeft();
        if (jsonPiece[0] == ',')
        {
            jsonPiece = jsonPiece[1..].SkipInsignificantSymbolsLeft();
        }
        return jsonPiece;
    }

    public static JsonPiece SkipToPropertyValue(this JsonPiece jsonPiece)
    {
        jsonPiece = jsonPiece.SkipInsignificantSymbolsLeft();

        var firstSymbol = jsonPiece[0];

        if (firstSymbol != ':')
        {
            throw new JsonException();
        }

        jsonPiece = jsonPiece[1..].SkipInsignificantSymbolsLeft();

        var result = jsonPiece;

        return result;
    }

    public static JsonPiece SkipInsignificantSymbolsLeft(this JsonPiece piece)
    {
        int length = piece.Length;
        int i = 0;
        for (; i < length; i++)
        {
            var c = piece[i];
            bool isEmptySymbol = c.IsEmptyCharacter();
            if (!isEmptySymbol)
            {
                break;
            }
        }
        return piece[i..];
    }

    public static JsonPiece TakeUntilEmpty(this JsonPiece jsonPiece)
    {
        int i = 0;
        for (; i < jsonPiece.Length; i++)
        {
            var c = jsonPiece[i];
            if (c.IsEmptyCharacter())
            {
                break;
            }
        }
        return jsonPiece[..i];
    }

    public static JsonPiece RemoveQuotedValue(this JsonPiece jsonPiece, out JsonPiece inQuotes)
    {
        var firstSymbol = jsonPiece[0];

        if (firstSymbol != '"')
        {
            throw new JsonException();
        }

        int i = 1;
        bool isClosed = false;
        bool isEscaped = false;
        int length = jsonPiece.Length;
        for (; i < length; i++)
        {
            var c = jsonPiece[i];
            if (c == '\\' || isEscaped)
            {
                isEscaped = !isEscaped;
            }
            else if (c == '"')
            {
                isClosed = true;
                break;
            }
        }
        if (!isClosed)
        {
            throw new JsonException();
        }

        inQuotes = jsonPiece[1..i];

        return jsonPiece[(i + 1)..];
    }

    static JsonPiece SkipOverClosedBracket(this JsonPiece jsonPiece, char bracketSymbol)
    {
        int i = 1;
        var currentCharacter = jsonPiece[i];
        do
        {
            if (currentCharacter == '"')
            {
                jsonPiece = jsonPiece[i..].RemoveQuotedValue(out var _);
                i = 0;
                currentCharacter = jsonPiece[i];
                continue;
            }
            currentCharacter = jsonPiece[++i];
        }
        while (currentCharacter != bracketSymbol);

        return jsonPiece[(i + 1)..];
    }

    public static JsonPiece SkipValue(this JsonPiece jsonPiece)
    {
        var firstSymbol = jsonPiece[0];

        if (firstSymbol == '"')
        {
            return jsonPiece.RemoveQuotedValue(out var _);
        }

        if (firstSymbol == '{')
        {
            return jsonPiece.SkipOverClosedBracket('}');
        }

        if (firstSymbol == '[')
        {
            return jsonPiece.SkipOverClosedBracket(']');
        }

        int j = 0;
        char c;
        do
        {
            c = jsonPiece[j++];
        }
        while (!c.IsClosingCharacter());

        return jsonPiece[(j - 1)..];
    }

    public static JsonPiece RemoveStringValue(this JsonPiece jsonPiece, out string result)
    {
        var firstSymbol = jsonPiece[0];

        if (firstSymbol != '"')
        {
            throw new JsonException();
        }

        Span<int> escaped = stackalloc int[jsonPiece.Length];
        int escapedCount = 0;

        int i = 1;
        bool isClosed = false;
        bool isEscaped = false;
        int length = jsonPiece.Length;
        for (; i < length; i++)
        {
            var c = jsonPiece[i];
            if (c == '\\' || isEscaped)
            {
                isEscaped = !isEscaped;
                if (isEscaped)
                {
                    escaped[escapedCount++] = i;
                }
            }
            else if (c == '"')
            {
                isClosed = true;
                break;
            }
        }
        if (!isClosed)
        {
            throw new JsonException();
        }
        if (escapedCount == 0)
        {
            result = new(jsonPiece[1..i]);
            return jsonPiece[(i + 1)..];
        }

        result = CopyToStringWithoutEscapedCharacters(jsonPiece[..i], escaped, escapedCount);

        return jsonPiece[(i + 1)..];
    }

    static string CopyToStringWithoutEscapedCharacters(JsonPiece jsonPiece, Span<int> escapedLots, int escapedCount)
    {
        int length = jsonPiece.Length;

        Span<char> result = stackalloc char[length - escapedCount - 1];

        int currentLot = 0;
        int currentEscapedIndex = 0;
        int currentEscaped = escapedLots[currentEscapedIndex++];

        for (int j = 1; j < length; j++)
        {
            bool isOnEscapeCharacter = j == currentEscaped;
            if (isOnEscapeCharacter)
            {
                currentEscaped = escapedLots[currentEscapedIndex++];
                continue;
            }
            var c = jsonPiece[j];
            result[currentLot++] = c;
        }

        return new(result);
    }

    public static JsonPiece ReadPrimitiveValue(this JsonPiece jsonPiece)
    {
        int i = 0;
        for (; i < jsonPiece.Length; i++)
        {
            var c = jsonPiece[i];
            if (c.IsClosingCharacter() || c.IsEmptyCharacter())
            {
                break;
            }
        }
        return jsonPiece[0..i];
    }

    public static JsonPiece RemoveArrayValues<T>(this JsonPiece json, ReadValue<T> getValue, out List<T> values)
    {
        json = json.SkipInsignificantSymbolsLeft();

        if (json[0] != '[')
        {
            throw new JsonException();
        }

        json = json[1..];

        json = json.SkipInsignificantSymbolsLeft();

        if (json[0] == ']')
        {
            values = new List<T>();
            return json[1..];
        }

        var result = new List<T>(5);

        bool isClosed;

        do
        {
            var c = json[0];

            if (c == ',')
            {
                json = json[1..].SkipInsignificantSymbolsLeft();
            }

            isClosed = c == ']';

            if (!isClosed)
            {
                json = getValue(json, out var item).SkipInsignificantSymbolsLeft();
                result.Add(item);
            }
        }
        while (!isClosed);

        values = result;
        return json[1..];
    }

    public static JsonPiece EnterObject(this JsonPiece jsonPiece)
    {
        jsonPiece = jsonPiece.SkipInsignificantSymbolsLeft();
        if (jsonPiece[0] != '{')
        {
            throw new JsonException();
        }
        return jsonPiece[1..].SkipInsignificantSymbolsLeft();
    }

    static bool IsClosingCharacter(this char c) => c == ',' || c == '}';
    static bool IsEmptyCharacter(this char c) => c == '\t' || c == '\r' || c == '\n' || c == ' ';

    public static JsonPiece RemoveNumber<T>(this JsonPiece jsonPiece, out T number)
        where T : INumber<T>
    {
        var numberPiece = jsonPiece.ReadPrimitiveValue();

        number = T.Parse(numberPiece, System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat);

        return jsonPiece[numberPiece.Length..];
    }
}
