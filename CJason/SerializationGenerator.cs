using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CJason
{
    public static class SerializationGenerator
    {
        public const string SerializeToMethodName = "SerializeTo";
        public const string SerializeMethodName = "Serialize";

        public static IEnumerable<IPropertySymbol> GetSerializableProperties(ITypeSymbol type) =>
            type.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.GetMethod != null && p.DeclaredAccessibility == Accessibility.Public);

        static string PutSerializationToSpan(
            ITypeSymbol type,
            SerializationSettings serializationSettings = null,
            string valueVariableName = "item",
            string spanVariableName = "span",
            string actualLengthVariableName = "actualLength"
        )
        {
            var ss = serializationSettings ?? new SerializationSettings();
            var code = new StringBuilder();
            code.AppendLine($"{actualLengthVariableName} = 2;");
            code.AppendLine("int current = 1;");
            if (type.IsPrimitive())
            {
                if (type.ValueIsInQuotes())
                {
                    code.AppendLine(
                        $@"current = $""{{""\""{valueVariableName}\""""}}"".AsSpan().CopiedTo({spanVariableName});
                        {actualLengthVariableName} += current;"
                    );
                }
                else
                {
                    code.AppendLine(
                        $@"current = {valueVariableName}.ToString().AsSpan().CopiedTo({spanVariableName});
                        {actualLengthVariableName} += current;"
                    );
                }
            }
            else
            {
                code.AppendLine($"{spanVariableName}[0] = '{{';");
                code.AppendLine($"{spanVariableName} = {spanVariableName}[1..];");
                var serializedProperties = GetSerializableProperties(type).ToArray();
                for (int i = 0; i < serializedProperties.Length; i++)
                {
                    var isLast = i == serializedProperties.Length - 1;
                    var isFirst = i == 0;
                    var property = serializedProperties[i];
                    var propertyType = property.Type;
                    bool isPrimitive = propertyType.IsPrimitive();
                    var propertyName = property.Name;
                    var jsonLength = $"{propertyName}JsonLength";

                    var checkNull = propertyType.IsReferenceType;
                    var checkDefault = isPrimitive && !checkNull;

                    string appendedPropertyName = $"\\\"{(ss.LowerPropertyCase ? propertyName.LowerFirstLetter() : propertyName)}\\\":";
                    int appendedPropertyNameLength = propertyName.Length + 3;

                    if (checkDefault || checkNull)
                    {
                        if (checkNull)
                        {
                            code.AppendLine(
                                $"if ({valueVariableName}.{propertyName} is not null) {{"
                            );
                        }
                        else if (checkDefault)
                        {
                            code.AppendLine(
                                $"if ({valueVariableName}.{propertyName} != default) {{"
                            );
                        }
                    }

                    code.AppendLine(
                        $"\"{appendedPropertyName}\".AsSpan().CopyTo({spanVariableName});"
                    );
                    code.AppendLine($"{actualLengthVariableName} += {appendedPropertyNameLength};");
                    code.AppendLine(
                        $"{spanVariableName} = {spanVariableName}[{appendedPropertyNameLength}..];"
                    );

                    if (isPrimitive)
                    {
                        if (propertyType.ValueIsInQuotes())
                        {
                            code.AppendLine(
                                $@"current = $""\""{{{valueVariableName}.{propertyName}}}\"""".AsSpan().CopiedTo({spanVariableName});"
                            );
                        }
                        else
                        {
                            code.AppendLine(
                                $@"current = {valueVariableName}.{propertyName}.ToString().AsSpan().CopiedTo({spanVariableName});"
                            );
                        }
                    }
                    else
                    {
                        code.AppendLine(
                            $@"{valueVariableName}.{propertyName}.{SerializeToMethodName}({spanVariableName}, out current);"
                        );
                    }
                    if (!isLast)
                    {
                        code.AppendLine($"{spanVariableName}[current] = ',';");
                        code.AppendLine($"current++;");
                    }
                    code.AppendLine($"{spanVariableName} = {spanVariableName}[current..];");
                    code.AppendLine($"{actualLengthVariableName} += current;");

                    if (checkDefault || checkNull)
                    {
                        code.AppendLine("}");
                    }
                }

                code.AppendLine($"{spanVariableName}[0] = '}}';");
            }

            return code.ToString();
        }

        public static string GenerateSerializationMethodsEtc(
            ITypeSymbol type,
            string valueVariableName = "item",
            string spanVariableName = "span",
            string actualLengthVariableName = "actualLength"
        )
        {
            string typeString = type.ToDisplayString();
            var code =
                $@"
{JsonLengthCalculationsGenerator.RenderTypeLengthStruct(type)}

{JsonLengthCalculationsGenerator.RenderCalculateJsonLengthMethod(type)}

static int {JsonLengthCalculationsGenerator.CalculateJsonLengthMethodName}(this Span<{typeString}> items)
{{
    var result = 2;
    var l = items.Length;
    for (int i = 0; i < l; i++)
    {{
        var item = items[i];
        result += {JsonLengthCalculationsGenerator.CalculateJsonLengthMethodName}(item);
    }}
    result += l - 1;
    return result;
}}

static int {JsonLengthCalculationsGenerator.CalculateJsonLengthMethodName}(this IEnumerable<{typeString}> items)
    => items.EnsureSpan().{JsonLengthCalculationsGenerator.CalculateJsonLengthMethodName}();

public static void {SerializeToMethodName}(
        this {typeString} {valueVariableName},
        Span<char> {spanVariableName},
        out int {actualLengthVariableName} 
    )
{{
    {PutSerializationToSpan(type, null, valueVariableName, spanVariableName, actualLengthVariableName)} 
}}

public static string {SerializeMethodName}(this {typeString} {valueVariableName})
{{
    var length = {JsonLengthCalculationsGenerator.CalculateJsonLengthMethodName}({valueVariableName});
    Span<char> resultSpan = stackalloc char[length];
    {valueVariableName}.{SerializeToMethodName}(resultSpan, out int actualLength);
    resultSpan = resultSpan[..actualLength];
    var result = new string(resultSpan);
    return result;
}}

public static void {SerializeToMethodName}(
        this Span<{typeString}> items,
        Span<char> targetSpan,
        out int jsonLength
    )
{{
    jsonLength = 1;
    targetSpan[0] = '[';
    targetSpan = targetSpan[1..];
    var l = items.Length;
    for (int i = 0; i < l; i++)
    {{
        var item = items[i];
        item.{SerializeToMethodName}(targetSpan, out var itemLength);
        if (i < l - 1)
        {{
            targetSpan[itemLength] = ',';
            itemLength++;
        }}
        jsonLength += itemLength;
        targetSpan = targetSpan[itemLength..];        
    }}
    targetSpan[0] = ']';
    jsonLength++;
}}

public static void {SerializeToMethodName}(this IEnumerable<{typeString}> source, Span<char> targetSpan, out int jsonLength)
    => source.EnsureSpan().{SerializeToMethodName}(targetSpan, out jsonLength);

public static string {SerializeMethodName}(this Span<{typeString}> items)
{{
    var jsonLength = {JsonLengthCalculationsGenerator.CalculateJsonLengthMethodName}(items);
    Span<char> spanResult = stackalloc char[jsonLength];

    items.{SerializeToMethodName}(spanResult, out var actualLength);

    spanResult = spanResult[..actualLength];

    var result = new string(spanResult);

    return result;
}}

public static string {SerializeMethodName}(this IEnumerable<{typeString}> source)
    => source.EnsureSpan().{SerializeMethodName}();
";

            return code;
        }
    }
}
