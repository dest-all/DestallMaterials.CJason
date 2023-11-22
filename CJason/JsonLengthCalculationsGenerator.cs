using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CJason
{
    public static class JsonLengthCalculationsGenerator
    {
        public const string CalculateJsonLengthMethodName = "CalculateJsonLength";

        public static int GetConstantLength(ITypeSymbol type)
        {
            type = type.UnderNullable();

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
                    var len = GetConstantLength(p.Type);
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
                    var propertyConstLength = GetConstantLength(propertyType);
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

        static void EnsurePresent<T>(this List<T> items, T item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
            }
        }

        public static string RenderCalculateJsonLengthMethod(this ITypeSymbol type)
        {
            var structName = TypeLengthStructName(type);

            var head =
                $@"static int {CalculateJsonLengthMethodName}(this {type.ToDisplayString()} item)";

            var properties = SerializationGenerator
                .GetSerializableProperties(type)
                .Select(p => (p, GetConstantLength(p.Type)));

            var propertiyLengthsSetting = properties.Select(p =>
            {
                var property = p.Item1;
                var constantLength = p.Item2;
                var lengthVar = $"item_{property.Name}_Length";
                string lengthVarAssignment;
                bool canBeNull = p.p.Type.IsReferenceType;
                if (constantLength < 1)
                {
                    lengthVarAssignment = $"var item_{property.Name} = item.{property.Name};\n" +
                    $"var {lengthVar} = {(canBeNull ? $"item_{property.Name} is null ? 0 : " : "")} ({property.Name.Length + 3} + {property.Type.CalculateJsonLengthExpression($"item_{property.Name}")});";
                }
                else 
                {
                    lengthVarAssignment = $"int {lengthVar} = item.{property.Name} == default ? 0 : ({property.Name.Length + 3} + {constantLength});";
                }

                return (lengthVar, lengthVarAssignment);
            }).ToArray();

            var result =
                $@"{head}
{{
    {propertiyLengthsSetting.Select(t => t.Item2).Join("\n")}
    var result = {propertiyLengthsSetting.Length} - 1 + {propertiyLengthsSetting.Select(t=>t.lengthVar).Join(" + ")};
    return result;
}}";

            return result;
        }

        public static bool ValueIsInQuotes(this ITypeSymbol type) => new Type[]
        {
            typeof(DateTime), typeof(TimeSpan), typeof(string), typeof(char)
        }.Any(t => type.Is(t)) || type.ToDisplayString() == "System.DateOnly";


        public static string CalculateJsonLengthExpression(this ITypeSymbol type, string variable)
        {
            string result;

            var constantLength = type.GetConstantLength();

            if (constantLength != -1)
            {
                return constantLength.ToString();
            }

            if (type.Is<string>())
            {
                return $"{variable}.Length * 2 + 2";
            }

            if (type.IsKeyValuePairs(out var key, out var value))
            {
                string valueVariable = $"{variable}_value";
                string keyVariable = $"{variable}_key";
                var calculateValueLength = CalculateJsonLengthExpression(value, valueVariable);
                var calculateKeyLength = CalculateJsonLengthExpression(key, keyVariable);

                result = $"{variable}.{CalculateJsonLengthMethodName}({keyVariable} => {calculateKeyLength}, {valueVariable} => {calculateValueLength})";

                return result;
            }
            if (type.IsEnumerable(out var underEnumerable))
            {
                var constItemLength = underEnumerable.GetConstantLength();

                if (constItemLength != -1)
                {
                    return $@"
    {{
        var itemsCount = {variable}.Count();
        if (itemsCount == 0)
        {{
            return 2;
        }}
        return itemsCount * ({constantLength} + 1) - 1 + 2;
    }}";
                }

                var itemVariable = $"{variable}_item";
                var calculateItemLength = CalculateJsonLengthExpression(underEnumerable, itemVariable);

                result = $"{variable}.{CalculateJsonLengthMethodName}({itemVariable} => {calculateItemLength})";

                return result;
            }

            result = $"{variable}.{CalculateJsonLengthMethodName}()";

            return result;

            throw new NotImplementedException($"Can't calculate length of {type.ToDisplayString()} type variable.");
        }

    }
}
