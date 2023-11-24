using JsonPiece = System.ReadOnlySpan<char>;
using System.Text.Json;
using System;

namespace CJason.Provision;

public static class TimeDeserializationExtensions
{
    public static JsonPiece Remove(this JsonPiece json, out TimeSpan timeSpan)
    {
        if (json[0] != '"')
        {
            throw new JsonException();
        }

        json = json[1..];

        var numbers = json.ReadSeparatedNumbers();

        bool isNegative = numbers.Item2 < 0;

        timeSpan = isNegative ? 
            new TimeSpan(numbers.Item2, -numbers.Item3, -numbers.Item4, -numbers.Item5) : 
            new TimeSpan(numbers.Item2, numbers.Item3, numbers.Item4, numbers.Item5);

        return json[numbers.Item1];
    }

    public static JsonPiece Remove(this JsonPiece json, out DateTimeOffset dateTimeOffset)
    {
        if (json[0] != '"')
        {
            throw new JsonException();
        }

        json = json[1..];

        var (pastDateTime,
           year,
           month,
           day,
           hour,
           minute,
           second) = json.ReadSeparatedNumbers();

        json = json[pastDateTime];

        var offsetSign = json[0];

        json = json[1..];

        var (pastOffset,
            hourOffset,
            minuteOffset,
            _, _, _, _) = json.ReadSeparatedNumbers();

        if (offsetSign == '-')
        {
            hourOffset *= -1;
            minuteOffset *= -1;
        }

        var offset = new TimeSpan(hourOffset, minuteOffset, 0);

        dateTimeOffset = new DateTimeOffset(year, month, day, hour, minute, second, offset);

        return json[pastOffset];
    }

    public static JsonPiece Remove(this JsonPiece json, out DateTime dateTime)
    {
        if (json[0] != '"')
        {
            throw new JsonException();
        }

        json = json[1..];

        var (pastQuotes,
            year,
            month,
            day,
            hour,
            minute,
            second) = json.ReadSeparatedNumbers();

        dateTime = new DateTime(year, month, day, hour, minute, second);

        return json[pastQuotes];
    }


    public static (Range, int, int, int, int, int, int) ReadSeparatedNumbers(this JsonPiece json)
    {
        const int char0 = '0';

        bool isNegative = json[0] == '-';
        if (isNegative)
        {
            json = json[1..];
        }

        Span<int> r = stackalloc int[6];

        int currentNumberIndex = 0;

        int numberStartedAt = 0;
        int i = 0;
        var l = json.Length;
        char c;
        do
        {
            c = json[i++];

            if (c == '.' || c == ':' || c == '/' || c == '"' || c == 'T' || c == ' ' || i == l)
            {
                int multiplier = 1;
                int currentNumber = 0;
                int j = i == l ? i - 1 : i - 2;
                for (; j >= numberStartedAt; j--)
                {
                    var numberChar = json[j];
                    currentNumber += (numberChar - char0) * multiplier;
                    multiplier *= 10;
                }

                bool isFirstNumber = currentNumberIndex == 0;
                r[currentNumberIndex++] = isNegative && isFirstNumber ? -currentNumber : currentNumber;
                numberStartedAt = i;
                continue;
            }
        }
        while (i < l && c != '"' && currentNumberIndex < r.Length);

        Range past = i == l ? (i - 1).. : isNegative ? (i+1).. : i..;
        var result = (past, r[0], r[1], r[2], r[3], r[4], r[5]);

        return result;
    }
}
