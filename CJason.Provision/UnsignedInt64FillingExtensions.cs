namespace CJason.Provision;

public static class UnsignedInt64FillingExtensions
{
    const int char0 = '0';

    public static Span<char> FillWith(this Span<char> span, ulong number)
    {
        var digitsNumber = GetDigitsNumber(number);

        int i = 0;
        for (; i < digitsNumber; i++)
        {
            var remainder = (int)(number % 10);
            char c = (char)(char0 + remainder);
            number /= 10;

            span[digitsNumber - i - 1] = c;
        }

        return span[i..];
    }

    static byte GetDigitsNumber(ulong n)
    {
        if (n <= ulong.MaxValue && n >= 10000000000000000000)
        {
            return 20;
        }

        byte result = 1;
        ulong divisor = 10;
        while (divisor < n)
        {
            divisor *= 10;
            result++;
        }
        return result;
    }
}
