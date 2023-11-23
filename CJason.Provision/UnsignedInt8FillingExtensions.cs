namespace CJason.Provision;

public static class UnsignedInt8FillingExtensions
{
    const int char0 = '0';
    const string _maxUInt8String = "255"; // byte.MaxValue.ToString()

    public static Span<char> FillWith(this Span<char> span, byte number)
    {
        var digitsNumber = GetDigitsNumber(number);

        int i = 0;
        for (; i < digitsNumber; i++)
        {
            var remainder = number % 10;
            char c = (char)(char0 + remainder);
            number = (byte)(number / 10);

            span[digitsNumber - i - 1] = c;
        }

        return span[i..];
    }

    static byte GetDigitsNumber(byte n)
    {
        if (n <= byte.MaxValue && n > 99)
        {
            return 3;
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
