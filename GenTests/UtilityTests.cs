using CJason.Provision;
using Newtonsoft.Json;
using JsonPiece = System.ReadOnlySpan<char>;

namespace GenTests;

public class UtilityTests
{
    [Test]
    public void DeserializeValue_DateTime_1()
    {
        var dateTime = DateTime.Today;

        JsonPiece json = $" :  \"{dateTime}\" }}";

        json.SkipToPropertyValue().RemoveQuotedValue(j => DateTime.Parse(j), out var parsedValue);

        Assert.That(parsedValue, Is.EqualTo(dateTime));
    }

    [Test]
    public void Serialize_EscapedString()
    {
        const string shouldBeEscaped = "\" \t \n \r \\";

        var ser1 = System.Text.Json.JsonSerializer.Serialize(shouldBeEscaped);
        var ser2 = JsonConvert.SerializeObject(shouldBeEscaped);

        Span<char> span = stackalloc char[100];
        span.FillWithQuoted(shouldBeEscaped);
        var ser3 = new string(new string(span).TakeWhile(c => c != '\0').ToArray());

        var des1 = System.Text.Json.JsonSerializer.Deserialize<string>(ser1);
        var des2 = System.Text.Json.JsonSerializer.Deserialize<string>(ser2);
        var des3 = System.Text.Json.JsonSerializer.Deserialize<string>(ser3);

        Assert.AreEqual(des1, des2);
        Assert.AreEqual(des2, des3);
    }

    [Test]
    public void DeserializeValue_String_0()
    {

        JsonPiece input = @" : "" \"" \"" trtret \\\"" \""""    ,  ";

        input.SkipToPropertyValue().RemoveQuotedValue(chars => new string(chars), out var readValue);

        string expected = @" "" "" trtret \"" """;

        Assert.That(readValue, Is.EqualTo(expected));
    }

    [Test]
    public void DeserializeValue_DateTime_2()
    {
        var dateTime = DateTime.Today;

        JsonPiece json = $" :  \"{dateTime}\" }}";

        json.SkipToPropertyValue().RemoveQuotedValue(chars => new string(chars), out var quotedValue);

        var parsedValue = DateTime.Parse(quotedValue);

        Assert.That(parsedValue, Is.EqualTo(dateTime));
    }

    [Test]
    public void Deserialize_Boolean()
    {
        bool value = true;

        JsonPiece json = $" : {value} , ";

        var flagString = json.SkipToPropertyValue().RemovePrimitiveValue(bool.Parse, out var flag);

        Assert.That(flag, Is.EqualTo(value));
    }

    [Test]
    public void Deserialize_StringArray()
    {
        var (s1, s2, s3, s4, s5) = ("34543543", "dfgdfgfd", "fdgfdgf", "-0-0-", "2324");

        JsonPiece json = $" :  [ \"{s1}\", \t\n \"{s2}\" \n, \n \"{s3}\", \"{s4}\", \"{s5}\" ]";

        json.SkipToPropertyValue().RemoveArray<string>(JsonDeserializationUtilities.RemoveString, out var strings);

        Assert.Contains(s1, strings);
        Assert.Contains(s2, strings);
        Assert.Contains(s3, strings);
        Assert.Contains(s4, strings);
        Assert.Contains(s5, strings);
    }

    [Test]
    public void Deserialize_PropertyAndValue()
    {
        const string property = "Property";

        const int value = 15;

        JsonPiece propertyDeserialized;
        int valueDeserialized;

        JsonPiece json = $" \t \"{property}\" \t:\t\n\r {value} \t";

        json
            .SkipInsignificantSymbolsLeft()
            .RemovePropertyName(out propertyDeserialized)
            .SkipToPropertyValue()
            .RemovePrimitiveValue(j => int.Parse(j), out valueDeserialized);

        Assert.That(new string(propertyDeserialized), Is.EqualTo(property));
        Assert.That(valueDeserialized, Is.EqualTo(value));
    }

    [Test]
    public void Deserialize_SeveralProperties()
    {
        var (p1, v1, p2, v2, p3, v3) =
            ("Name", "Igor", "Age", 26, "BirthDate", new DateTime(1996, 12, 06));

        JsonPiece json = $@"{{
    ""{p1}"" : ""{v1}"",
    ""{p2}"" : {v2},
    ""{p3}"" : ""{v3}""
}}";

        JsonPiece dp1, dp2, dp3;
        string dv1;
        int dv2;
        DateTime dv3;

        json
            .EnterObject()
            .RemovePropertyName(out dp1)
            .SkipToPropertyValue()
            .RemoveString(out dv1)
            .SkipToPropertyName()
            .RemovePropertyName(out dp2)
            .SkipToPropertyValue()
            .RemovePrimitiveValue(j => int.Parse(j), out dv2)
            .SkipToPropertyName()
            .RemovePropertyName(out dp3)
            .SkipToPropertyValue()
            .RemoveQuotedValue(j => DateTime.Parse(j), out dv3);

        Assert.That(new string(dp1), Is.EqualTo(p1));
        Assert.That(new string(dv1), Is.EqualTo(v1));
        Assert.That(new string(dp2), Is.EqualTo(p2));
        Assert.That(dv2, Is.EqualTo(v2));
        Assert.That(new string(dp3), Is.EqualTo(p3));
        Assert.That(dv3, Is.EqualTo(v3));
    }

    [Test]
    public void SkipValue_Primitive()
    {
        JsonPiece valueJson = "23534534543.32387435}";

        var skipped = valueJson.SkipValue();

        Assert.That(skipped.Length, Is.EqualTo(1));
    }

    [Test]
    public void SkipValue_String()
    {
        JsonPiece valueJson = "\"23534534543.32387435\",";

        var skipped = valueJson.SkipValue();

        Assert.That(skipped.Length, Is.EqualTo(1));
    }

    [Test]
    public void SkipValue_Object()
    {
        var obj = new { Name = "Bob } ,, }", Surname = "Terminator" };

        var jsonObj = System.Text.Json.JsonSerializer.Serialize(obj);

        JsonPiece json = $"{jsonObj}-";

        var skipped = json.SkipValue();

        Assert.That(skipped.Length, Is.EqualTo(1));
    }

    [Test]
    public void SkipValue_Array()
    {
        JsonPiece json = "[3453453, \"34543785634\", ewrewrw, \"''''\"]-";
        var skipped = json.SkipValue();

        Assert.That(skipped.Length, Is.EqualTo(1));
    }

    [Test]
    public void SwitchJsonPiece()
    {
        const string example = "example";
        JsonPiece chars = example;

        short index = chars switch
        {
            "12345" => -1,
            example => 1,
            _ => -1
        };

        Assert.AreEqual(1, index);
    }

    [Test]
    public void DeserializeEscapedString()
    {
        const string shouldBeEscaped = "\" \t \n \r \\";

        Span<char> span = stackalloc char[100];
        span.FillWithQuoted(shouldBeEscaped);
        var ser = new string(new string(span).TakeWhile(c => c != '\0').ToArray());

        var serStandard = JsonConvert.SerializeObject(shouldBeEscaped);

        var desStandard = JsonConvert.DeserializeObject<string>(new(ser));
        ser.AsSpan().RemoveQuotedValue(j => new string(j), out string des);

        Assert.AreEqual(shouldBeEscaped, desStandard);
        Assert.AreEqual(desStandard, des);
        Assert.AreEqual(serStandard, ser);
    }


    [Test]
    public void Deserialize_Char()
    {
        const char c = '\t';

        Span<char> span = stackalloc char[9];
        span.FillWithQuoted(c.ToString());

        var json = new string(span).AsSpan();

        json.RemoveQuotedValue(j => j[0], out char cDes);

        var serStandard = JsonConvert.SerializeObject(c);

        var cDesStandard = JsonConvert.DeserializeObject<Char>(new string(json));

        Assert.AreEqual(c, cDes);
        Assert.AreEqual(cDes, cDesStandard);
    }

    [Test]
    public void String_ToString()
    {
        const string a = "34r3432432dsf";

        var b = a.ToString();

        Assert.IsTrue(object.ReferenceEquals(b, a));
    }



    [Test]
    public void Number_Fill()
    {
        int[] numbers = [int.MinValue, int.MaxValue, 0, 324, 12, 34353454];

        Span<char> span1Orig = stackalloc char[100];
        string str2 = "";

        var span1 = span1Orig;

        foreach (var number in numbers)
        {
            span1 = span1.FillWith(number);
            str2 += number.ToString();
        }

        var str1 = new string(new string(span1Orig).TakeWhile(c => c != '\0').ToArray());

        Assert.AreEqual(str1, str2);
    }
}
