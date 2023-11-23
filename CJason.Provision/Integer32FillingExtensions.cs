namespace CJason.Provision;

public static class Integer32FillingExtensions
{
    const int char0 = '0';
    const string _minIntString = "-2147483648";
    public static Span<char> FillWith(this Span<char> span, int number)
    {
        bool isPositive = number >= 0;

        int dev = -1;

        if (!isPositive)
        {
            if (number == int.MinValue)
            {
                _minIntString.AsSpan().CopyTo(span);
                return span[11..];
            }
            number = -number;
            dev = 0;
        }

        var digitsNumber = GetDigitsNumber(number);

        int i = 0;
        for (; i < digitsNumber; i++)
        {
            var remainder = number % 10;
            char c = (char)(char0 + remainder);
            number /= 10;

            span[digitsNumber - i + dev] = c;
        }

        if (!isPositive)
        {
            span[0] = '-';
        }

        return span[i..];
    }

    static byte GetDigitsNumber(int n)
    {
        if (n <= int.MaxValue && n >= 1000000000)
        {
            return 10;
        }

        byte result = 1;
        var divisor = 10;
        while (divisor < n)
        {
            divisor *= 10;
            result++;
        }
        return result;
    }
}

public static class Int64FillingExtensions
{
    const int char0 = '0';
    const string _minInt64String = "-9223372036854775808";

    public static Span<char> FillWith(this Span<char> span, long number)
    {
        bool isPositive = number >= 0;

        int dev = -1;

        if (!isPositive)
        {
            if (number == long.MinValue)
            {
                _minInt64String.AsSpan().CopyTo(span);
                return span[19..];
            }
            number = -number;
            dev = 0;
        }

        var digitsNumber = GetDigitsNumber(number);

        int i = 0;
        for (; i < digitsNumber; i++)
        {
            var remainder = (int)(number % 10);
            char c = (char)(char0 + remainder);
            number = number / 10;

            span[digitsNumber - i + dev] = c;
        }

        if (!isPositive)
        {
            span[0] = '-';
        }

        return span[i..];
    }

    static byte GetDigitsNumber(long n)
    {
        if (n <= long.MaxValue && n >= 1000000000000000000)
        {
            return 19;
        }

        byte result = 1;
        var divisor = 10L;
        while (divisor < n)
        {
            divisor *= 10;
            result++;
        }
        return result;
    }
}
