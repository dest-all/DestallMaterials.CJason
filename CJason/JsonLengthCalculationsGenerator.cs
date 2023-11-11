using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CJason
{
    public static class JsonLengthCalculationsGenerator
    {
        public const string CalculateJsonLengthMethodName = "CalculateJsonLength";

        public static int ConstantLength(ITypeSymbol type)
        {
            if (type.IsPrimitive())
            {
                var constLength = type.GetConstantLength();
                if (constLength > 0)
                {
                    return constLength;
                }
                return -1;
            }
            var result = 0;
            var allAreConst = SerializationGenerator
                .GetSerializableProperties(type)
                .All(p =>
                {
                    var len = ConstantLength(p.Type);
                    result += len;
                    return len < 1;
                });
            if (allAreConst)
            {
                return result;
            }
            return -1;
        }

        public static string TypeLengthStructName(ITypeSymbol type) =>
            type.ToDisplayString().Replace(".", "_") + "_Length";

        public static string RenderTypeLengthStruct(ITypeSymbol type)
        {
            string structName = TypeLengthStructName(type);

            var properties = SerializationGenerator.GetSerializableProperties(type);

            (string Name, SpecialType SpecialType) a = (type.Name, type.SpecialType);
            var lengthsRequired = properties.Select(p => (p.Name, p.Type));
            int typeConstantLength = 2;
            List<string> variableLengthFields = new List<string>() { "0" };
            string lengthsRendered = lengthsRequired
                .Select(l =>
                {
                    var line = $"public int {l.Name}";
                    ITypeSymbol propertyType = l.Type;
                    var propertyConstLength = ConstantLength(propertyType);
                    if (propertyConstLength > 0)
                    {
                        line += $" => {propertyConstLength}";
                        typeConstantLength += propertyConstLength;
                    }
                    else
                    {
                        variableLengthFields.Add(l.Name);
                    }

                    typeConstantLength += 2 + 1 + l.Name.Length + 1;

                    if (ValueIsInQuotes(l.Type))
                    {
                        typeConstantLength += 2;
                    }

                    if (l.Type.IsEnumerable(out var _))
                    {
                        typeConstantLength += 2;
                    }

                    return line + ";";
                })
                .Join("\n");

            typeConstantLength--; // Subtract the last comma

            var definition =
                $@"
ref struct {structName}
{{
    public int Length => {typeConstantLength} + {variableLengthFields.Join(" + ")};
    {lengthsRendered}

    public static implicit operator int({structName} asStruct) => asStruct.Length;
}}";

            return definition;
        }

        public static string RenderCalculateJsonLengthMethod(ITypeSymbol type)
        {
            var structName = TypeLengthStructName(type);

            var head =
                $@"static {structName} {CalculateJsonLengthMethodName}(this {type.ToDisplayString()} item)";
            var properties = SerializationGenerator
                .GetSerializableProperties(type)
                .Where(p => ConstantLength(p.Type) < 1);
            var propertiesSetting = properties.Select(p =>
            {
                var propType = p.Type;
                var propName = p.Name;
                if (propType.IsPrimitive())
                {
                    if (propType.Is<string>())
                    {
                        return $"{propName} = item.{propName}.Length";
                    }
                    return $"{propName} = item.{propName}.ToString().Length";
                }
                if (propType.IsEnumerable(out var underEnumerable))
                {
                    return $"{propName} = item.{propName}.EnsureSpan().CalculateJsonLength()";
                }
                return $"{propName} = {CalculateJsonLengthMethodName}(item.{propName})";
            });
            var result =
                $@"{head}
            => new()
            {{
                {propertiesSetting.Join(",\n")}
            }};";

            return result;
        }

        public static string CalculateSpanJsonLengthMethod(ITypeSymbol typeSymbol) =>
            $@"
public static int {CalculateJsonLengthMethodName}(this System.Span<{typeSymbol.ToDisplayString()}> items)
{{
    var result = 0;
    var len = items.Length;
    for (int i = 0; i < len; i++)
    {{
        var itemLength = items[i].{CalculateJsonLengthMethodName}();
        result += itemLength;
    }}
    return result + 2; // + two square brackets
}}";

        public static bool ValueIsInQuotes(this ITypeSymbol type) =>
            type.Is<DateTime>() || type.Is<string>();
    }
}
