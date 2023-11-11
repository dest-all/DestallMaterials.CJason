namespace SampleNamespace {
        public class Father
        {
            public string Name { get; set; }
            public int Age { get; set; }

            public Child[] Children { get; set; }
        }

        public class Child 
        {
            public int Age { get; set; }
            public string Name { get; set; }
        }
    }
        namespace SampleNamespace 
        {
            public static class ArtificialStringifier
            {
                public static Span<T> EnsureSpan<T>(this IEnumerable<T> source)
                {
                    if (source is T[] span)
                    {
                        return span;
                    }
                    if (source is List<T> list)
                    {
                        return System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
                    }
                    var array = source.ToArray();
                    return array;
                }

                public static int CopiedTo<T>(this Span<T> source, Span<T> target)
                {
                    source.CopyTo(target);
                    return source.Length;
                }

                public static int CopiedTo<T>(this ReadOnlySpan<T> source, Span<T> target)
                {
                    source.CopyTo(target);
                    return source.Length;
                }

                


ref struct SampleNamespace_Father_Length
{
    public int Length => 43 + 0 + Name + Children;
    public int Name;
public int Age => 11;
public int Children;

    public static implicit operator int(SampleNamespace_Father_Length asStruct) => asStruct.Length;
}

static SampleNamespace_Father_Length CalculateJsonLength(this SampleNamespace.Father item)
            => new()
            {
                Name = item.Name.Length,
Children = item.Children.EnsureSpan().CalculateJsonLength()
            };

static int CalculateJsonLength(this Span<SampleNamespace.Father> items)
{
    var result = 2;
    var l = items.Length;
    for (int i = 0; i < l; i++)
    {
        var item = items[i];
        result += CalculateJsonLength(item);
    }
    result += l - 1;
    return result;
}

static int CalculateJsonLength(this IEnumerable<SampleNamespace.Father> items)
    => items.EnsureSpan().CalculateJsonLength();

public static void SerializeTo(
        this SampleNamespace.Father item,
        Span<char> span,
        out int actualLength 
    )
{
    actualLength = 2;
int current = 1;
span[0] = '{';
span = span[1..];
if (item.Name is not null) {
"\"Name\":".AsSpan().CopyTo(span);
actualLength += 7;
span = span[7..];
current = $"\"{item.Name}\"".AsSpan().CopiedTo(span);
span[current] = ',';
current++;
span = span[current..];
actualLength += current;
}
if (item.Age != default) {
"\"Age\":".AsSpan().CopyTo(span);
actualLength += 6;
span = span[6..];
current = item.Age.ToString().AsSpan().CopiedTo(span);
span[current] = ',';
current++;
span = span[current..];
actualLength += current;
}
if (item.Children is not null) {
"\"Children\":".AsSpan().CopyTo(span);
actualLength += 11;
span = span[11..];
item.Children.SerializeTo(span, out current);
span = span[current..];
actualLength += current;
}
span[0] = '}';
 
}

public static string Serialize(this SampleNamespace.Father item)
{
    var length = CalculateJsonLength(item);
    Span<char> resultSpan = stackalloc char[length];
    item.SerializeTo(resultSpan, out int actualLength);
    resultSpan = resultSpan[..actualLength];
    var result = new string(resultSpan);
    return result;
}

public static void SerializeTo(
        this Span<SampleNamespace.Father> items,
        Span<char> targetSpan,
        out int jsonLength
    )
{
    jsonLength = 1;
    targetSpan[0] = '[';
    targetSpan = targetSpan[1..];
    var l = items.Length;
    for (int i = 0; i < l; i++)
    {
        var item = items[i];
        item.SerializeTo(targetSpan, out var itemLength);
        if (i < l - 1)
        {
            targetSpan[itemLength] = ',';
            itemLength++;
        }
        jsonLength += itemLength;
        targetSpan = targetSpan[itemLength..];        
    }
    targetSpan[0] = ']';
    jsonLength++;
}

public static void SerializeTo(this IEnumerable<SampleNamespace.Father> source, Span<char> targetSpan, out int jsonLength)
    => source.EnsureSpan().SerializeTo(targetSpan, out jsonLength);

public static string Serialize(this Span<SampleNamespace.Father> items)
{
    var jsonLength = CalculateJsonLength(items);
    Span<char> spanResult = stackalloc char[jsonLength];

    items.SerializeTo(spanResult, out var actualLength);

    spanResult = spanResult[..actualLength];

    var result = new string(spanResult);

    return result;
}

public static string Serialize(this IEnumerable<SampleNamespace.Father> source)
    => source.EnsureSpan().Serialize();



ref struct SampleNamespace_Child_Length
{
    public int Length => 29 + 0 + Name;
    public int Age => 11;
public int Name;

    public static implicit operator int(SampleNamespace_Child_Length asStruct) => asStruct.Length;
}

static SampleNamespace_Child_Length CalculateJsonLength(this SampleNamespace.Child item)
            => new()
            {
                Name = item.Name.Length
            };

static int CalculateJsonLength(this Span<SampleNamespace.Child> items)
{
    var result = 2;
    var l = items.Length;
    for (int i = 0; i < l; i++)
    {
        var item = items[i];
        result += CalculateJsonLength(item);
    }
    result += l - 1;
    return result;
}

static int CalculateJsonLength(this IEnumerable<SampleNamespace.Child> items)
    => items.EnsureSpan().CalculateJsonLength();

public static void SerializeTo(
        this SampleNamespace.Child item,
        Span<char> span,
        out int actualLength 
    )
{
    actualLength = 2;
int current = 1;
span[0] = '{';
span = span[1..];
if (item.Age != default) {
"\"Age\":".AsSpan().CopyTo(span);
actualLength += 6;
span = span[6..];
current = item.Age.ToString().AsSpan().CopiedTo(span);
span[current] = ',';
current++;
span = span[current..];
actualLength += current;
}
if (item.Name is not null) {
"\"Name\":".AsSpan().CopyTo(span);
actualLength += 7;
span = span[7..];
current = $"\"{item.Name}\"".AsSpan().CopiedTo(span);
span = span[current..];
actualLength += current;
}
span[0] = '}';
 
}

public static string Serialize(this SampleNamespace.Child item)
{
    var length = CalculateJsonLength(item);
    Span<char> resultSpan = stackalloc char[length];
    item.SerializeTo(resultSpan, out int actualLength);
    resultSpan = resultSpan[..actualLength];
    var result = new string(resultSpan);
    return result;
}

public static void SerializeTo(
        this Span<SampleNamespace.Child> items,
        Span<char> targetSpan,
        out int jsonLength
    )
{
    jsonLength = 1;
    targetSpan[0] = '[';
    targetSpan = targetSpan[1..];
    var l = items.Length;
    for (int i = 0; i < l; i++)
    {
        var item = items[i];
        item.SerializeTo(targetSpan, out var itemLength);
        if (i < l - 1)
        {
            targetSpan[itemLength] = ',';
            itemLength++;
        }
        jsonLength += itemLength;
        targetSpan = targetSpan[itemLength..];        
    }
    targetSpan[0] = ']';
    jsonLength++;
}

public static void SerializeTo(this IEnumerable<SampleNamespace.Child> source, Span<char> targetSpan, out int jsonLength)
    => source.EnsureSpan().SerializeTo(targetSpan, out jsonLength);

public static string Serialize(this Span<SampleNamespace.Child> items)
{
    var jsonLength = CalculateJsonLength(items);
    Span<char> spanResult = stackalloc char[jsonLength];

    items.SerializeTo(spanResult, out var actualLength);

    spanResult = spanResult[..actualLength];

    var result = new string(spanResult);

    return result;
}

public static string Serialize(this IEnumerable<SampleNamespace.Child> source)
    => source.EnsureSpan().Serialize();

            }
        }