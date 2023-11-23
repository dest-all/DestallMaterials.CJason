using JsonPiece = System.ReadOnlySpan<char>;
using System.Globalization;
using System.Numerics;
using System.Text.Json;

namespace CJason.Provision;

public delegate JsonPiece RemoveValue<T>(JsonPiece jsonPiece, out T parsedValue);

public delegate T ParseValue<T>(JsonPiece jsonPiece);

public static class JsonDeserializationUtilities
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

        jsonPiece = jsonPiece[(i + 1)..];

        return jsonPiece;
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

    public static JsonPiece RemoveQuotedValue<T>(this JsonPiece jsonPiece, ParseValue<T> parse, out T value)
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
            value = parse(jsonPiece[1..i]);
            return jsonPiece[(i + 1)..];
        }

        Span<char> parsedBuffer = stackalloc char[i - escapedCount - 1];

        CopyWithoutEscapedCharacters(jsonPiece[1..i], parsedBuffer);

        value = parse(parsedBuffer);

        return jsonPiece[(i + 1)..];
    }

    public static JsonPiece SkipOverClosedBracket(this JsonPiece jsonPiece, char bracketSymbol)
    {
        if (jsonPiece.IsEmpty)
        {
            throw new JsonException();
        }

        char openingBracket = bracketSymbol switch
        {
            ']' => '[',
            '}' => '{',
            '(' => ')',
            '<' => '>',
            _ => throw new InvalidOperationException($"'{bracketSymbol}' is not a bracket.")
        };

        int opens = 1;

        int i = 0;

        while (opens > 0 && i < jsonPiece.Length)
        {
            var currentCharacter = jsonPiece[i];
            if (currentCharacter == openingBracket)
            {
                opens++;
            }
            else if (currentCharacter == bracketSymbol)
            {
                opens--;
            }
            else if (currentCharacter == '"')
            {
                jsonPiece = jsonPiece[i..].RemoveQuotedValue(chars => default(string), out var _);
                i = -1;
            }
            i++;
        }

        if (opens > 0)
        {
            return "";
        }

        jsonPiece = jsonPiece[i..];

        return jsonPiece;
    }

    public static JsonPiece SkipValue(this JsonPiece jsonPiece)
    {
        var firstSymbol = jsonPiece[0];

        if (firstSymbol == '"')
        {
            return jsonPiece.RemoveQuotedValue(chars => default(string), out var _);
        }

        if (firstSymbol == '{')
        {
            return jsonPiece[1..].SkipOverClosedBracket('}');
        }

        if (firstSymbol == '[')
        {
            return jsonPiece[1..].SkipOverClosedBracket(']');
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

    static void CopyWithoutEscapedCharacters(JsonPiece source, Span<char> targetBuffer)
    {
        int length = source.Length;
        int dev = 0;
        var targetIndex = 0;
        for (int sourceIndex = 0; sourceIndex < length; sourceIndex++)
        {
            char c = source[sourceIndex];
            if (c == '\\')
            {
                targetBuffer[targetIndex] = source[++sourceIndex].ToEscapedCharacter();
            }
            else 
            {
                targetBuffer[targetIndex] = c;
            }
            targetIndex++;
        }
    }

    public static JsonPiece RemovePrimitiveValue<T>(this JsonPiece jsonPiece, ParseValue<T> parseValue, out T value)
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

        value = parseValue(jsonPiece[0..i]);

        return jsonPiece[i..];
    }

    public static JsonPiece RemoveList<T>(this JsonPiece json, RemoveValue<T> removeValue, out List<T> values)
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

        var result = new List<T>(json.Length / 70);

        bool isClosed;

        do
        {
            json = json.SkipInsignificantSymbolsLeft();
            var c = json[0];

            if (c == ',')
            {
                json = json[1..].SkipInsignificantSymbolsLeft();
            }

            isClosed = c == ']';

            if (!isClosed)
            {
                json = removeValue(json, out var item).SkipInsignificantSymbolsLeft();
                result.Add(item);
            }
        }
        while (!isClosed);

        values = result;
        return json[1..];
    }

    public static JsonPiece RemoveArray<T>(this JsonPiece json, RemoveValue<T> removeValue, out T[] arr)
    {
        json = json.RemoveList(removeValue, out var list);
        arr = list.ToArray();
        return json;
    }

    public static JsonPiece RemoveDictionary<TKey, TValue>(
        this JsonPiece json,
        RemoveValue<TKey> removeKey,
        RemoveValue<TValue> removeValue,
        out Dictionary<TKey, TValue> dictionary)
    {
        dictionary = new Dictionary<TKey, TValue>(5);

        json = json
            .EnterObject();

        while (json[0] != '}')
        {
            json = removeValue(
                removeKey(json.SkipToPropertyName(), out var key)
                .SkipToPropertyValue(),
                out var value)
                .SkipInsignificantSymbolsLeft();

            dictionary.Add(key, value);
        }

        json = json.SkipOverClosedBracket('}');

        return json;
    }

    public static JsonPiece RemoveStringKeyDictionary<TValue>(this JsonPiece jsonPiece, RemoveValue<TValue> removeValue, out Dictionary<string, TValue> dict)
        => jsonPiece.RemoveDictionary<string, TValue>(RemoveString, removeValue, out dict);

    public static JsonPiece EnterObject(this JsonPiece jsonPiece)
    {
        jsonPiece = jsonPiece.SkipInsignificantSymbolsLeft();
        if (jsonPiece[0] != '{')
        {
            throw new JsonException();
        }
        return jsonPiece[1..].SkipInsignificantSymbolsLeft();
    }

    static bool IsClosingCharacter(this char c) => c == ',' || c == '}' || c == ']';
    static bool IsEmptyCharacter(this char c) => c == '\t' || c == '\r' || c == '\n' || c == ' ' || c == '\0';

    static T ParseNumber<T>(JsonPiece jsonPiece)
        where T : INumber<T> => T.Parse(jsonPiece, System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat);

    public static JsonPiece RemoveNumber<T>(this JsonPiece jsonPiece, out T number)
        where T : INumber<T> => jsonPiece.RemovePrimitiveValue(ParseNumber<T>, out number);

    public static JsonPiece RemoveString(this JsonPiece jsonPiece, out string str)
    {
        jsonPiece = jsonPiece.RemoveQuotedValue(chars => new string(chars), out str);
        return jsonPiece;
    }

    static char ToEscapedCharacter(this char c)
        => c switch
        {
            '"' => '"',
            'n' => '\n',
            't' => '\t',
            'r' => '\r',
            '\\' => '\\',
            '0' => '\0',
            _ => ' '
        };

    public static bool TryReadNull<T>(this JsonPiece jsonPiece, out T result)
    {
        var l = jsonPiece.Length;
        result = default;
        if (l > 3 && jsonPiece[0] == 'n' && jsonPiece[1] == 'u' && jsonPiece[2] == 'l' && jsonPiece[3] == 'l')
        {
            var nextSymbol = jsonPiece[4];
            if (l == 4 || nextSymbol.IsClosingCharacter() || nextSymbol.IsEmptyCharacter())
            {
                return true;
            }
        }
        return false;
    }
}
