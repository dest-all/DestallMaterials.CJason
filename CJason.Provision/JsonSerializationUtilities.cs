using System.Text.Json.Serialization;
using JsonPiece = System.Span<char>;

namespace CJason.Provision;

public delegate JsonPiece FillWithObject<T>(JsonPiece json, T obj);
public delegate int CalculateItemLength<T>(T item);

public static class JsonSerializationUtilities
{
    public static int CalculateJsonLength<TValue>(this IEnumerable<KeyValuePair<string, TValue>> keyValuePairs, CalculateItemLength<TValue> calculateValueLength)
    {
        var result = 2;
        foreach (var (key, value) in keyValuePairs)
        {
            result += key.Length + 2 + 1 + 1;
            var valueLength = calculateValueLength(value);
            result += valueLength;
        }
        return result == 2 ? 2 : (result - 1);
    }

    public static int CalculateJsonLength<T>(this ReadOnlySpan<T> items, CalculateItemLength<T> calculateValueLength)
    {
        var result = 2;
        var l = items.Length;
        for (int i = 0; i < l; i++)
        {
            var value = items[i];
            result += calculateValueLength(value) + 1;
        }
        return result == 2 ? 2 : (result - 1);
    }

    public static int CalculateJsonLength<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs, CalculateItemLength<TKey> calculateKeyLength, CalculateItemLength<TValue> calculateValueLength)
    {
        var result = 2;
        foreach (var (key, value) in keyValuePairs)
        {
            result += calculateKeyLength(key) + 2 + 1 + 1;
            var valueLength = calculateValueLength(value);
            result += valueLength;
        }
        return result == 2 ? 2 : (result - 1);
    }

    public static int CalculateJsonLength<T>(this IEnumerable<T> items, CalculateItemLength<T> calculateValueLength)
        => items.EnsureReadOnlySpan().CalculateJsonLength(calculateValueLength);

    public static JsonPiece FillWith<T>(this JsonPiece json, Span<T> items, FillWithObject<T> fillWith)
    {
        json[0] = '[';
        var l = items.Length;
        if (l == 0)
        {
            json[1] = ']';
            return json[2..];
        }
        json = json[1..];
        for (int i = 0; i < l; i++)
        {
            var item = items[i];
            json = fillWith(json, item);
            if (i < l - 1)
            {
                json[0] = ',';
                json = json[1..];
            }
        }
        json[0] = ']';
        return json[1..];
    }

    public static JsonPiece FillWith<T>(this JsonPiece jsonPiece, IEnumerable<T> items, FillWithObject<T> fillWith)
        => jsonPiece.FillWith(items.EnsureSpan(), fillWith);

    public static JsonPiece FillWith<TKey, TValue>(
        this JsonPiece jsonPiece,
        IEnumerable<KeyValuePair<TKey, TValue>> keyValues,
        FillWithObject<TKey> fillWithKey,
        FillWithObject<TValue> fillWithValue)
    {
        jsonPiece[0] = '{';
        jsonPiece = jsonPiece[1..];

        bool isFirst = true;

        foreach (var (key, value) in keyValues)
        {
            if (!isFirst)
            {
                jsonPiece[0] = ',';
                jsonPiece = jsonPiece[1..];
            }
            else 
            {
                isFirst = false;
            }
            jsonPiece = fillWithKey(jsonPiece, key);
            jsonPiece[0] = ':';
            jsonPiece = jsonPiece[1..];
            jsonPiece = fillWithValue(jsonPiece, value);
        }

        jsonPiece[0] = '}';

        jsonPiece = jsonPiece[1..];

        return jsonPiece;
    }

    public static JsonPiece FillWithQuoted(this JsonPiece json, ReadOnlySpan<char> item)
    {
        json[0] = '"';
        json = json[1..];
        int dev = 0;
        var l = item.Length;
        int i = 0;
        for (; i < l; i++)
        {
            var c = item[i];
            var lot = i + dev;
            if (c == '"' || c == '\\')
            {
                json[lot] = '\\';
                json[lot] = c;
                dev++;
            }
            else 
            {
                json[lot] = c;
            }
        }
        json[i + dev] = '"';
        return json[(i + dev + 1)..];
    }

    public static JsonPiece FillWith(this JsonPiece jsonPiece, ReadOnlySpan<char> item)
        => jsonPiece[item.CopiedTo(jsonPiece)..];
}