namespace CJason.Provision;

public static class DateTimeFilling
{
    const int char0 = '0';

    public static Span<char> FillWith(this Span<char> span, DateTime dateTime)
    {
        var year = dateTime.Year;
        var month = dateTime.Month;
        var day = dateTime.Day;

        var hour = dateTime.Hour;
        var minute = dateTime.Minute;
        var second = dateTime.Second;

        // Year
        span[0] = (char)(char0 + year / 1000);
        span[1] = (char)(char0 + year / 100 % 10);
        span[2] = (char)(char0 + year / 10 % 10);
        span[3] = (char)(char0 + year % 10);
        span[4] = '.';

        // Month
        span[5] = (char)(char0 + month / 10);
        span[6] = (char)(char0 + month % 10);
        span[7] = '.';

        // Day
        span[8] = (char)(char0 + day / 10);
        span[9] = (char)(char0 + day % 10);
        span[10] = 'T';

        // Hour
        span[11] = (char)(char0 + hour / 10);
        span[12] = (char)(char0 + hour % 10);
        span[13] = ':';

        // Minute
        span[14] = (char)(char0 + minute / 10);
        span[15] = (char)(char0 + minute % 10);
        span[16] = ':';

        // Second
        span[17] = (char)(char0 + second / 10);
        span[18] = (char)(char0 + second % 10);

        return span[19..];
    }

    public static Span<char> FillWith(this Span<char> span, DateTimeOffset dateTimeOffset)
    {
        var dateTimePart = dateTimeOffset.DateTime;
        span = span.FillWith(dateTimePart);

        var offsetHours = Math.Abs(dateTimeOffset.Offset.Hours);
        var offsetMinutes = Math.Abs(dateTimeOffset.Offset.Minutes);

        // Offset
        span[0] = dateTimeOffset.Offset.Hours >= 0 ? '+' : '-';
        span[1] = (char)(char0 + offsetHours / 10);
        span[2] = (char)(char0 + offsetHours % 10);
        span[3] = ':';
        span[4] = (char)(char0 + offsetMinutes / 10);
        span[5] = (char)(char0 + offsetMinutes % 10);

        return span[6..];
    }

    public static Span<char> FillWith(this Span<char> span, TimeSpan timeSpan)
    {
        bool isNegative = timeSpan.Ticks < 0;

        if (isNegative)
        {
            if (timeSpan == TimeSpan.MinValue)
            {
                const string minValueString = "-10675199.02:48:05";
                minValueString.AsSpan().CopyTo(span);
                return span[18..];
            }
            timeSpan = TimeSpan.FromTicks(-timeSpan.Ticks);
            span[0] = '-';
            span = span[1..];
        }

        var days = timeSpan.Days;
        var hours = timeSpan.Hours;
        var minutes = timeSpan.Minutes;
        var seconds = timeSpan.Seconds;

        // Days
        var daysLength = FillWithNumber(span, days, GetDigitsNumber(days));
        span[daysLength] = '.';

        // Hours
        var hoursLength = FillWithNumber(span[(daysLength + 1)..], hours, 2);
        span[daysLength + 1 + hoursLength] = ':';

        // Minutes
        var minutesLength = FillWithNumber(span[(daysLength + 1 + hoursLength + 1)..], minutes, 2);
        span[daysLength + 1 + hoursLength + 1 + minutesLength] = ':';

        var secondsAt = daysLength + 1 + hoursLength + 1 + minutesLength + 1;

        // Seconds
        FillWithNumber(span[secondsAt..], seconds, 2);

        return span[(secondsAt + 2)..];
    }

    private static int FillWithNumber(Span<char> span, int number, int length)
    {
        for (int i = length - 1; i >= 0; i--)
        {
            span[i] = (char)(char0 + number % 10);
            number /= 10;
        }

        return length;
    }

    static byte GetDigitsNumber(int n)
    {
        if (n <= int.MaxValue && n >= 1000000000)
        {
            return 10;
        }

        byte result = 1;
        var divisor = 10;
        while (divisor <= n)
        {
            divisor *= 10;
            result++;
        }
        return result;
    }

}
