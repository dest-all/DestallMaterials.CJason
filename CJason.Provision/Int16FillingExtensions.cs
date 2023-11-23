namespace CJason.Provision;

public static class Int16FillingExtensions
{
    const int char0 = '0';
    const string _minInt16String = "-32768";

    public static Span<char> FillWith(this Span<char> span, short number)
    {
        bool isPositive = number >= 0;

        int dev = -1;

        if (!isPositive)
        {
            if (number == short.MinValue)
            {
                _minInt16String.AsSpan().CopyTo(span);
                return span[6..];
            }
            number = (short)-number;
            dev = 0;
        }

        var digitsNumber = GetDigitsNumber(number);

        int i = 0;
        for (; i < digitsNumber; i++)
        {
            var remainder = number % 10;
            char c = (char)(char0 + remainder);
            number = (short)(number / 10);

            span[digitsNumber - i + dev] = c;
        }

        if (!isPositive)
        {
            span[0] = '-';
        }

        return span[i..];
    }

    static byte GetDigitsNumber(short n)
    {
        if (n <= short.MaxValue && n >= 10000)
        {
            return 5;
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
