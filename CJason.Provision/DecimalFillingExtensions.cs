namespace CJason.Provision;

public static class DecimalFillingExtensions
{
    const int char0 = '0';
    const string _minDecimalString = "-79228162514264337593543950335"; // decimal.MinValue.ToString()

    public static Span<char> FillWith(this Span<char> span, double number)
        => span.FillWith((decimal)number);

    public static Span<char> FillWith(this Span<char> span, float number)
        => span.FillWith((decimal)number);

    public static Span<char> FillWith(this Span<char> span, decimal number)
    {
        if (number == 0)
        {
            span[0] = '0';
            return span[1..];
        }

        bool isNegative = false;

        // Handle negative numbers
        if (number < 0)
        {
            isNegative = true;
            if (number == decimal.MinValue)
            {
                _minDecimalString.AsSpan().CopyTo(span);
                return span[30..];
            }
            number = -number;
        }

        int integerPart = (int)number;
        decimal fractionPart = number - integerPart;

        int intDigits = GetDigitsNumber(integerPart);
        int fracDigits = GetFractionDigits(fractionPart);

        if (isNegative)
        {
            span[0] = '-';
        }

        // Fill the integer part
        int i = 0;
        int dev = isNegative ? 1 : 0;
        for (; i < intDigits; i++)
        {
            var remainder = integerPart % 10;
            char c = (char)(char0 + remainder);
            integerPart = integerPart / 10;

            int at = intDigits - i - 1 + dev;

            span[at] = c;
        }

        if (fracDigits == 0)
        {
            return span[(i + dev)..];
        }

        span[intDigits + dev] = '.';
        i = intDigits + 1 + dev;

        // Fill the fractional part
        while (fracDigits > 0)
        {
            fractionPart *= 10;
            var digit = (int)fractionPart;
            char c = (char)(char0 + digit);
            fractionPart -= digit;

            span[i] = c;
            i++;
            fracDigits--;
        }

        return span[i..];
    }

    const decimal _smallestLongestDecimal = (decimal)1000000000000 * 10000000000000000;

    static byte GetDigitsNumber(decimal n)
    {
        if (n <= decimal.MaxValue && n >= _smallestLongestDecimal)
        {
            return 29;
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

    static int GetFractionDigits(decimal n)
    {
        int count = 0;
        while (n % 1 != 0 && count < 28) // Maximum fraction digits for decimal type
        {
            n *= 10;
            count++;
        }
        return count;
    }
}