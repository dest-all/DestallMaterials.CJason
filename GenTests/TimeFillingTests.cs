using CJason.Provision;

namespace GenTests;

[TestFixture]
public class TimeFillingTests
{
    [Test]
    public void DateTimeFillingExtensions_FillWith()
    {
        DateTime dateTime = new DateTime(2023, 11, 23, 15, 30, 45);
        Span<char> buffer = new char[19];
        buffer.FillWith(dateTime);

        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("2023.11.23T15:30:45", result);

        // Verify with Parse
        DateTime parsedDateTime = DateTime.Parse(result);
        Assert.AreEqual(dateTime, parsedDateTime);
    }

    [Test]
    public void DateTimeOffsetFillingExtensions_FillWith()
    {
        DateTimeOffset dateTimeOffset = new DateTimeOffset(2023, 11, 23, 15, 30, 45, TimeSpan.FromHours(2));
        Span<char> buffer = new char[25];
        buffer.FillWith(dateTimeOffset);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("2023.11.23T15:30:45+02:00", result);

        // Verify with Parse
        DateTimeOffset parsedDateTimeOffset = DateTimeOffset.Parse(result);
        Assert.AreEqual(dateTimeOffset, parsedDateTimeOffset);
    }

    [Test]
    public void TimeSpanFillingExtensions_FillWith()
    {
        TimeSpan timeSpan = new TimeSpan(2, 15, 30, 45);
        Span<char> buffer = new char[16];
        buffer.FillWith(timeSpan);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("2.15:30:45", result);

        // Verify with Parse
        TimeSpan parsedTimeSpan = TimeSpan.Parse(result);
        Assert.AreEqual(timeSpan, parsedTimeSpan);
    }

    // Edge cases from here.

    [Test]
    public void DateTimeFillingExtensions_FillWith_SpecificValue()
    {
        DateTime dateTime = new DateTime(2023, 11, 23, 15, 30, 45);
        Span<char> buffer = new char[19];
        buffer.FillWith(dateTime);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("2023.11.23T15:30:45", result);

        // Verify with Parse
        DateTime parsedDateTime = DateTime.Parse(result);
        Assert.AreEqual(dateTime, parsedDateTime);
    }

    [Test]
    public void DateTimeFillingExtensions_FillWith_MinValue()
    {
        DateTime minDateTime = DateTime.MinValue;
        Span<char> buffer = new char[19];
        buffer.FillWith(minDateTime);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("0001.01.01T00:00:00", result);

        // Verify with Parse
        DateTime parsedMinDateTime = DateTime.Parse(result);
        Assert.AreEqual(minDateTime, parsedMinDateTime);
    }

    [Test]
    public void DateTimeFillingExtensions_FillWith_MaxValue()
    {
        DateTime maxDateTime = DateTime.MaxValue;
        Span<char> buffer = new char[19];
        buffer.FillWith(maxDateTime);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("9999.12.31T23:59:59", result);
    }

    [Test]
    public void DateTimeFillingExtensions_FillWith_DefaultValue()
    {
        DateTime defaultDateTime = default(DateTime);
        Span<char> buffer = new char[19];
        buffer.FillWith(defaultDateTime);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("0001.01.01T00:00:00", result);

        // Verify with Parse
        DateTime parsedDefaultDateTime = DateTime.Parse(result);
        Assert.AreEqual(defaultDateTime, parsedDefaultDateTime);
    }

    [Test]
    public void DateTimeOffsetFillingExtensions_FillWith_SpecificValue()
    {
        DateTimeOffset dateTimeOffset = new DateTimeOffset(2023, 11, 23, 15, 30, 45, TimeSpan.FromHours(2));
        Span<char> buffer = new char[25];
        buffer.FillWith(dateTimeOffset);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("2023.11.23T15:30:45+02:00", result);

        // Verify with Parse
        DateTimeOffset parsedDateTimeOffset = DateTimeOffset.Parse(result);
        Assert.AreEqual(dateTimeOffset, parsedDateTimeOffset);
    }

    [Test]
    public void DateTimeOffsetFillingExtensions_FillWith_MinValue()
    {
        DateTimeOffset minDateTimeOffset = DateTimeOffset.MinValue;
        Span<char> buffer = new char[25];
        buffer.FillWith(minDateTimeOffset);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("0001.01.01T00:00:00+00:00", result);

        // Verify with Parse
        DateTimeOffset parsedMinDateTimeOffset = DateTimeOffset.Parse(result);
        Assert.AreEqual(minDateTimeOffset, parsedMinDateTimeOffset);
    }

    [Test]
    public void DateTimeOffsetFillingExtensions_FillWith_MaxValue()
    {
        DateTimeOffset maxDateTimeOffset = DateTimeOffset.MaxValue;
        Span<char> buffer = new char[25];
        buffer.FillWith(maxDateTimeOffset);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("9999.12.31T23:59:59+00:00", result);
    }

    [Test]
    public void DateTimeOffsetFillingExtensions_FillWith_DefaultValue()
    {
        DateTimeOffset defaultDateTimeOffset = default(DateTimeOffset);
        Span<char> buffer = new char[25];
        buffer.FillWith(defaultDateTimeOffset);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("0001.01.01T00:00:00+00:00", result);

        // Verify with Parse
        DateTimeOffset parsedDefaultDateTimeOffset = DateTimeOffset.Parse(result);
        Assert.AreEqual(defaultDateTimeOffset, parsedDefaultDateTimeOffset);
    }

    [Test]
    public void TimeSpanFillingExtensions_FillWith_SpecificValue()
    {
        TimeSpan timeSpan = new TimeSpan(2, 15, 30, 45);
        Span<char> buffer = new char[16];
        buffer.FillWith(timeSpan);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("2.15:30:45", result);

        // Verify with Parse
        TimeSpan parsedTimeSpan = TimeSpan.Parse(result);
        Assert.AreEqual(timeSpan, parsedTimeSpan);
    }

    [Test]
    public void TimeSpanFillingExtensions_FillWith_MinValue()
    {
        TimeSpan minTimeSpan = TimeSpan.MinValue;
        Span<char> buffer = new char[19];
        buffer.FillWith(minTimeSpan);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("-10675199.02:48:05", result);
    }

    [Test]
    public void NegativeTimeSpan()
    {
        TimeSpan value = TimeSpan.FromSeconds(-234238);

        Span<char> buffer = new char[19];
        buffer.FillWith(value);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual(value, TimeSpan.Parse(result));
        Assert.AreEqual(value.ToString(), result);
    }

    [Test]
    public void TimeSpanFillingExtensions_FillWith_MaxValue()
    {
        TimeSpan maxTimeSpan = TimeSpan.MaxValue;
        Span<char> buffer = new char[17];
        buffer.FillWith(maxTimeSpan);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual(maxTimeSpan.ToString()[..^8], result);
    }

    [Test]
    public void TimeSpanFillingExtensions_FillWith_DefaultValue()
    {
        TimeSpan defaultTimeSpan = default(TimeSpan);
        Span<char> buffer = new char[16];
        buffer.FillWith(defaultTimeSpan);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual("0.00:00:00", result);

        // Verify with Parse
        TimeSpan parsedDefaultTimeSpan = TimeSpan.Parse(result);
        Assert.AreEqual(defaultTimeSpan, parsedDefaultTimeSpan);
    }

    [Test]
    public void TimeSpan_CustomTimeSpan()
    {
        TimeSpan timeSpan = new TimeSpan(10, 10, 10, 10);
        Span<char> buffer = new char[16];
        buffer.FillWith(timeSpan);
        var result = buffer.ToStringUntilTerminator();

        Assert.AreEqual(timeSpan.ToString(), result);

        // Verify with Parse
        TimeSpan parsedDefaultTimeSpan = TimeSpan.Parse(result);
        Assert.AreEqual(timeSpan, parsedDefaultTimeSpan);
    }
}

public class TimeDeserializationTests
{
    [Test]
    public void RemoveDateTime()
    {
        Span<char> str = stackalloc char[50];

        var dateTime = new DateTime(2023, 10, 10, 10, 10, 10);

        str.FillWithQuoted(dateTime)[0] = '-';

        var readonlyStr = (ReadOnlySpan<char>)str;

        var rem = readonlyStr.Remove(out DateTime des);

        Assert.AreEqual(dateTime, des);
        Assert.AreEqual('-', rem[0]);
    }

    [Test]
    public void RemoveTimeSpan()
    {
        TimeSpan[] timeSpans = [
            new TimeSpan(10, 10, 10, 10), 
            new(-10, 10, 10, 10), 
            new(-10, -10, -10, -10), 
            default, 
            TimeSpan.MinValue, 
            TimeSpan.MaxValue
            ];

        foreach (var time in timeSpans)
        {
            Span<char> str = stackalloc char[50];

            str.FillWithQuoted(time)[0] = '-';

            var readonlyStr = (ReadOnlySpan<char>)str;

            var rem = readonlyStr.Remove(out TimeSpan des);

            Assert.AreEqual((int)time.TotalSeconds, (int)des.TotalSeconds);
            Assert.AreEqual('-', rem[0]);
        }
    }

    static bool MatchToSeconds(TimeSpan first, TimeSpan second)
        => first.TotalSeconds == second.TotalSeconds;
}