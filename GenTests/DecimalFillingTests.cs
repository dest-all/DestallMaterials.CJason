using CJason.Provision;

namespace GenTests;

[TestFixture]
public class DecimalFillingTests
{
    [Test]
    public void FillWith_PositiveInteger()
    {
        decimal number = 1234567890;
        Span<char> buffer = new char[100];

        var result = buffer.FillWith(number);
        var expectedResult = number.ToString();

        Assert.AreEqual(expectedResult, buffer.ToStringUntilTerminator());
        Assert.AreEqual(expectedResult.Length, buffer.Length - result.Length);
    }

    [Test]
    public void FillWith_NegativeInteger()
    {
        decimal number = -987654321;
        Span<char> buffer = new char[100];

        var result = buffer.FillWith(number);
        var expectedResult = number.ToString();

        Assert.AreEqual(expectedResult, buffer.ToStringUntilTerminator());
        Assert.AreEqual(expectedResult.Length, buffer.Length - result.Length);
    }

    [Test]
    public void FillWith_PositiveDecimal()
    {
        decimal number = 1234567890.987654321m;
        Span<char> buffer = new char[100];

        var result = buffer.FillWith(number);
        var expectedResult = number.ToString();

        Assert.AreEqual(expectedResult, buffer.ToStringUntilTerminator());
        Assert.AreEqual(expectedResult.Length, buffer.Length - result.Length);
    }

    [Test]
    public void FillWith_NegativeDecimal()
    {
        decimal number = -987654321.546731237m;
        Span<char> buffer = new char[100];

        var result = buffer.FillWith(number);
        var expectedResult = number.ToString();

        Assert.AreEqual(expectedResult, buffer.ToStringUntilTerminator());
        Assert.AreEqual(expectedResult.Length, buffer.Length - result.Length);
    }

    [Test]
    public void FillWith_Zero()
    {
        decimal number = 0;
        Span<char> buffer = new char[100];

        var result = buffer.FillWith(number);
        var expectedResult = number.ToString();

        Assert.AreEqual(expectedResult, buffer.ToStringUntilTerminator());
        Assert.AreEqual(expectedResult.Length, buffer.Length - result.Length);
    }

    [Test]
    public void FillWith_MinValue()
    {
        decimal number = decimal.MinValue;
        Span<char> buffer = new char[100];

        var result = buffer.FillWith(number);
        var expectedResult = number.ToString();

        Assert.AreEqual(expectedResult, buffer.ToStringUntilTerminator());
        Assert.AreEqual(expectedResult.Length, buffer.Length - result.Length);
    }

    [Test]
    public void TryIncreaseMax()
    {
        var number = 123.0000000000000000000000654321m;


    }
}
