using CJason.Provision;
using System.Runtime.InteropServices;
using System.Text.Json;
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
    public void Deserialize_NumberArray()
    {
        var (n1, n2, n3, n4, n5) = (1000, 400, 5, 18923, 343434);

        JsonPiece json = $" :  [ {n1}, \t\n {n2} \n, \n {n3}, {n4}, {n5} ]";

        json.SkipToPropertyValue().RemoveArray<int>(JsonDeserializationUtilities.RemoveNumber<int>, out var ints);

        Assert.Contains(n1, ints);
        Assert.Contains(n2, ints);
        Assert.Contains(n3, ints);
        Assert.Contains(n4, ints);
        Assert.Contains(n5, ints);
    }

    [Test]
    public void Deserialize_NumberArray_2()
    {
        JsonPiece numbersArrayString = $"[1,1500]";

        numbersArrayString.RemoveArray<int>(JsonDeserializationUtilities.RemoveNumber<int>, out var numbers);


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
            .RemoveNumber(out valueDeserialized);

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
            .RemoveNumber(out dv2)
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

        var jsonObj = JsonSerializer.Serialize(obj);

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
}
