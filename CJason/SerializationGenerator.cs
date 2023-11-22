using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CJason
{
    public static class SerializationGenerator
    {
        public const string FillWithMethodName = "FillWith";
        public const string SerializeMethodName = "Serialize";

        public static IEnumerable<IPropertySymbol> GetSerializableProperties(ITypeSymbol type) =>
            type.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.GetMethod != null && p.DeclaredAccessibility == Accessibility.Public);

        public static IEnumerable<INamedTypeSymbol> GatherReferredTypes(this ITypeSymbol type)
        {
            var nts = type as INamedTypeSymbol;
            if (type.IsPrimitive())
            {
                yield break;
            }

            bool isSystemType = nts == null || type.ContainingNamespace.ToDisplayString().StartsWith("System");

            if (!isSystemType)
            {
                yield return nts;

                foreach (var propType in nts.GetMembers().OfType<IPropertySymbol>().SelectMany(p => p.Type.GatherReferredTypes()))
                {
                    yield return propType;
                }
            }
            else if (type.IsEnumerable(out var underEnumerable))
            {
                foreach (var dependency in underEnumerable.GatherReferredTypes())
                {
                    yield return dependency;
                }
            }
            else if (type.IsKeyValuePairs(out var key, out var value))
            {
                foreach (var dependency in key.GatherReferredTypes())
                {
                    yield return dependency;
                }
                foreach (var dependency in value.GatherReferredTypes())
                {
                    yield return dependency;
                }
            }
        }

        static string PutSerializationToSpan(
            ITypeSymbol type,
            SerializationSettings serializationSettings = null,
            string valueVariableName = "item",
            string spanVariableName = "span"
        )
        {
            var ss = serializationSettings ?? new SerializationSettings();
            var code = new StringBuilder();
            if (type.IsPrimitive())
            {
                if (type.ValueIsInQuotes())
                {
                    code.AppendLine(
                        $@"{spanVariableName} = {spanVariableName}[$""{{""\""{valueVariableName}\""""}}"".AsSpan().CopiedTo({spanVariableName})..];"
                    );
                }
                else
                {
                    code.AppendLine(
                        $@"{spanVariableName} = {spanVariableName}[{valueVariableName}.ToString().AsSpan().CopiedTo({spanVariableName})..];"
                    );
                }
            }
            else
            {
                code.AppendLine($"{spanVariableName}[0] = '{{';");
                code.AppendLine($"bool isFirst = true;");
                code.AppendLine($"{spanVariableName} = {spanVariableName}[1..];");
                var serializedProperties = GetSerializableProperties(type).ToArray();
                for (int i = 0; i < serializedProperties.Length; i++)
                {
                    var property = serializedProperties[i];
                    var propertyType = property.Type;
                    bool isPrimitive = propertyType.IsPrimitive();
                    var propertyName = property.Name;

                    var checkNull = propertyType.IsReferenceType || propertyType.NullableAnnotation == NullableAnnotation.Annotated;
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

                    code.AppendLine($@"if (isFirst)
{{
    isFirst = false;
}}
else 
{{
    {spanVariableName}[0] = ',';
    {spanVariableName} = {spanVariableName}[1..];
}}");

                    code.AppendLine(
                        $"\"{appendedPropertyName}\".AsSpan().CopyTo({spanVariableName});"
                    );

                    code.AppendLine(
                        $"{spanVariableName} = {spanVariableName}[{appendedPropertyNameLength}..];"
                    );

                    var propertyVariable = $"{valueVariableName}_{propertyName}";
                    code.AppendLine($@"
var {propertyVariable} = {valueVariableName}.{propertyName};
{spanVariableName} = {propertyType.PickFillingMethod(propertyVariable, spanVariableName)};
");

                    if (checkDefault || checkNull)
                    {
                        code.AppendLine("}");
                    }
                }

                code.AppendLine($"{spanVariableName}[0] = '}}';");
            }

            code.AppendLine($"{spanVariableName} = {spanVariableName}[1..];");

            return code.ToString();
        }

        static string PickFillingMethod(this ITypeSymbol variableType, string variable, string spanVariable)
        {
            variableType = variableType.UnderNullable();

            bool isPrimitive = variableType.IsPrimitive();
            if (isPrimitive)
            {
                if (variableType.ValueIsInQuotes())
                {
                    return $"{spanVariable}.FillWithQuoted({variable}.ToString())";
                }
                else
                {
                    return $"{spanVariable}.FillWith({variable}.ToString())";
                }
            }
            else if (variableType.IsKeyValuePairs(out var key, out var value))
            {
                string keyVariable = $"{variable}_key";
                string valueVariable = $"{variable}_value";
                string jsonPieceVariable = $"{spanVariable}_dictionarySpan";

                return $@"{spanVariable}.{FillWithMethodName}({variable}, 
    (Span<char> {jsonPieceVariable}, {key.ToDisplayString()} {keyVariable}) => {key.PickFillingMethod(keyVariable, jsonPieceVariable)},
    (Span<char> {jsonPieceVariable}, {value.ToDisplayString()} {valueVariable}) => {value.PickFillingMethod(valueVariable, jsonPieceVariable)})";

            }
            else if (variableType.IsEnumerable(out var underEnumerable))
            {
                string itemVariable = $"{variable}_item";
                string jsonPieceVariable = $"{spanVariable}_arraySpan";
                return $@"{spanVariable}.FillWith({variable}.EnsureSpan(), 
(Span<char> {jsonPieceVariable}, {underEnumerable.ToDisplayString()} {itemVariable}) => {underEnumerable.PickFillingMethod(itemVariable, jsonPieceVariable)})";
            }
            else 
            {
                return $"{spanVariable}.{FillWithMethodName}({variable})";
            }
        }

        public static string GenerateSerializationMethodsEtc(
            ITypeSymbol type,
            string valueVariableName = "item",
            string spanVariableName = "span"
        )
        {
            string typeString = type.ToDisplayString();

            var code =
                $@"
{JsonLengthCalculationsGenerator.RenderCalculateJsonLengthMethod(type)}

public static Span<char> {FillWithMethodName}(
        this Span<char> {spanVariableName},
        {typeString} {valueVariableName}
    )
{{
    {PutSerializationToSpan(type, null, valueVariableName, spanVariableName)};
    return {spanVariableName};
}}


public static string {SerializeMethodName}(this {typeString} {valueVariableName})
{{
    var length = {JsonLengthCalculationsGenerator.CalculateJsonLengthMethodName}({valueVariableName});
    Span<char> resultSpan = stackalloc char[length];
    resultSpan.{FillWithMethodName}({valueVariableName});
    var result = new string(resultSpan);
    return result;
}}


/*
public static Span<char> {FillWithMethodName}(this Span<char> span,
    IEnumerable<KeyValuePair<string, {typeString}>> keyValues)
{{
    span[0] = '{{';
    span = span[1..];
    bool isFirst = true;
    foreach (var (key, value) in keyValues)
    {{
        if (!isFirst)
        {{
            span[0] = ',';
            span = span[1..];
        }}
        else 
        {{
            isFirst = false;
        }}
        var spanKey = key.AsSpan();
        span[0] = '""';
        span = span[1..];
        spanKey.CopyTo(span);
        span = span[spanKey.Length..];
        span[0] = '""';
        span[1] = ':';
        span = span[2..];
        span = span.{FillWithMethodName}(value);
    }}
    span[0] = '}}';
    return span[1..];
}}

public static Span<char> {FillWithMethodName}(this Span<char> targetSpan, IEnumerable<{typeString}> source)
    => targetSpan.{FillWithMethodName}(source.EnsureSpan());

public static string {SerializeMethodName}(this Span<{typeString}> items)
{{
    var jsonLength = {JsonLengthCalculationsGenerator.CalculateJsonLengthMethodName}(items);
    Span<char> spanResult = stackalloc char[jsonLength];

    spanResult.{FillWithMethodName}(items);

    var result = new string(spanResult);

    return result;
}}

public static string {SerializeMethodName}(this IEnumerable<{typeString}> source)
    => source.EnsureSpan().{SerializeMethodName}();

*/
";

            return code;
        }
    }
}
