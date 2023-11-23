namespace CJason.Provision;

public static class UnsignedInt32FillingExtensions
{
    const int char0 = '0';
    public static Span<char> FillWith(this Span<char> span, uint number)
    {
        var digitsNumber = GetDigitsNumber(number);

        int i = 0;
        for (; i < digitsNumber; i++)
        {
            var remainder = number % 10;
            char c = (char)(char0 + remainder);
            number /= 10;

            span[digitsNumber - i - 1] = c;
        }

        return span[i..];
    }

    static byte GetDigitsNumber(uint n)
    {
        if (n <= uint.MaxValue && n >= 1000000000)
        {
            return 10;
        }

        byte result = 1;
        uint divisor = 10;
        while (divisor < n)
        {
            divisor *= 10;
            result++;
        }
        return result;
    }
}
