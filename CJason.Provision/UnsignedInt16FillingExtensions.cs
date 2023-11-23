namespace CJason.Provision;

public static class UnsignedInt16FillingExtensions
{
    const int char0 = '0';

    public static Span<char> FillWith(this Span<char> span, ushort number)
    {
        var digitsNumber = GetDigitsNumber(number);

        int i = 0;
        for (; i < digitsNumber; i++)
        {
            var remainder = number % 10;
            char c = (char)(char0 + remainder);
            number = (ushort)(number / 10);

            span[digitsNumber - i - 1] = c;
        }

        return span[i..];
    }

    static byte GetDigitsNumber(ushort n)
    {
        if (n <= ushort.MaxValue && n >= ushort.MaxValue / 10)
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
