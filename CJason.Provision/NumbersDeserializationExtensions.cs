using JsonPiece = System.ReadOnlySpan<char>;
using System.Text.Json;

namespace CJason.Provision;

public static class NumbersDeserializationExtensions
{
    const int char0 = '0';

    static (Range valueSpan, Range pastValue) ReadPrimitive(this JsonPiece json)
    {
        int i = 0;
        var l = json.Length;
        for (; i < l; i++)
        {
            var c = json[i];
            if (c.IsClosingCharacter())
            {
                return (0..i, i..);
            }
        }
        throw new JsonException();
    }

    public static JsonPiece Remove(this JsonPiece json, out byte result)
    {
        var (valueSpan, pastValue) = json.ReadPrimitive();

        var length = valueSpan.End.Value - valueSpan.Start.Value;

        result = 0;

        byte multiplier = 1;

        bool isNegative = json[0] == '-';

        int i0 = isNegative ? 1 : 0;

        for (int i = length - 1; i >= i0; i--)
        {
            var c = json[i];
            result += (byte)((c - char0) * multiplier);
            multiplier *= 10;
        }

        return json[pastValue];
    }

    public static JsonPiece Remove(this JsonPiece json, out int result)
    {
        var (valueSpan, pastValue) = json.ReadPrimitive();

        var length = valueSpan.End.Value - valueSpan.Start.Value;

        result = 0;

        var multiplier = 1;

        bool isNegative = json[0] == '-';

        int i0 = isNegative ? 1 : 0;

        for (int i = length - 1; i >= i0; i--)
        {
            var c = json[i];
            result += (c - char0) * multiplier;
            multiplier *= 10;
        }

        return json[pastValue];
    }

    public static JsonPiece Remove(this JsonPiece json, out decimal result)
    {
        var (valueSpan, pastValue) = json.ReadPrimitive();
        result = decimal.Parse(json[valueSpan]);
        return json[pastValue];
    }
    public static JsonPiece Remove(this JsonPiece json, out double result)
    {
        var (valueSpan, pastValue) = json.ReadPrimitive();
        result = double.Parse(json[valueSpan]);
        return json[pastValue];
    }

    public static JsonPiece Remove(this JsonPiece json, out float result)
    {
        var (valueSpan, pastValue) = json.ReadPrimitive();
        result = float.Parse(json[valueSpan]);
        return json[pastValue];
    }

    public static JsonPiece Remove(this JsonPiece json, out short result)
    {
        var (valueSpan, pastValue) = json.ReadPrimitive();

        var length = valueSpan.End.Value - valueSpan.Start.Value;

        result = 0;

        short multiplier = 1;

        bool isNegative = json[0] == '-';

        int i0 = isNegative ? 1 : 0;

        for (int i = length - 1; i >= i0; i--)
        {
            var c = json[i];
            result += (short)((c - char0) * multiplier);
            multiplier *= 10;
        }

        return json[pastValue];
    }

    public static JsonPiece Remove(this JsonPiece json, out long result)
    {
        var (valueSpan, pastValue) = json.ReadPrimitive();
        result = long.Parse(json[valueSpan]);
        return json[pastValue];
    }

    public static JsonPiece Remove(this JsonPiece json, out uint result)
    {
        var (valueSpan, pastValue) = json.ReadPrimitive();
        result = uint.Parse(json[valueSpan]);
        return json[pastValue];
    }
    public static JsonPiece Remove(this JsonPiece json, out ushort result)
    {
        var (valueSpan, pastValue) = json.ReadPrimitive();
        result = ushort.Parse(json[valueSpan]);
        return json[pastValue];
    }
    public static JsonPiece Remove(this JsonPiece json, out ulong result)
    {
        var (valueSpan, pastValue) = json.ReadPrimitive();
        result = ulong.Parse(json[valueSpan]);
        return json[pastValue];
    }


}
