using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CJason
{
    public static class DeserializationGenerator
    {
        public const string DeserializeFromMethodName = "DeserializeFrom";

        static IEnumerable<PropertyDeserializationVariableInfo> GetDeserializableVariables(this Compilation compilation, ITypeSymbol typeSymbol)
        {
            foreach (var property in typeSymbol.GetMembers()
                                                .OfType<IPropertySymbol>()
                                                .Where(p => p.SetMethod != null && p.DeclaredAccessibility == Accessibility.Public))
            {
                var valueImplementationType = compilation.PickValueImplementationType(property.Type);
                yield return new PropertyDeserializationVariableInfo
                {
                    Type = valueImplementationType.Item2,
                    Name = property.Name,
                    ConstructorOrder = -1,
                    PropertyTypeVariant = valueImplementationType.Item1
                };
            }
        }

        class PropertyDeserializationVariableInfo
        {
            public string Name;
            public PropertyTypeVariant PropertyTypeVariant;
            public ITypeSymbol Type;
            public short ConstructorOrder = -1;

            public bool IsConstructorArgument => ConstructorOrder > -1;
        }

        public static StringBuilder GenerateRemoveObjectMethod(
            this Compilation compilation,
            ITypeSymbol type,
            SerializationSettings settings = null,
            string valueVariableName = "item",
            string jsonVariableName = "json")
        {
            settings = settings ?? new SerializationSettings();

            var json = jsonVariableName;

            var properties = compilation.GetDeserializableVariables(type).ToArray();

            var typeString = type.ToDisplayString();

            var result = new StringBuilder($"public static System.ReadOnlySpan<char> RemoveObject(this System.ReadOnlySpan<char> json, out {typeString} {valueVariableName}) {{");

            foreach (var property in properties)
            {
                result.AppendLine($"{property.Type.ToDisplayString()} {property.Name.LowerFirstLetter()} = default;");
            }

            result.AppendLine($@"const int propsCount = {properties.Length};
int i = 0;
{json} = {json}.EnterObject();
while (i < propsCount)
{{
    {json} = {json}.SkipInsignificantSymbolsLeft();
    if ({json}[0] == '}}')
    {{
        break;
    }}
    
    {json} = {json}
        .SkipToPropertyName()
        .RemovePropertyName(out var propertyName)
        .SkipToPropertyValue();

    short propertyIndex = propertyName switch 
    {{
        {properties.Select((p, i) => $"\"{(settings.LowerPropertyCase ? p.Name.LowerFirstLetter() : p.Name)}\" => {i},").Join("\n")}
        _ => -1
    }};

    switch (propertyIndex)
    {{
        {properties.Select((p, i) => $@"case {i}: 
{{ 
    {compilation.RemoveValueCode(p.Type, p.PropertyTypeVariant, p.Name.LowerFirstLetter(), json)} 
    break;
}}").Join("\n")}
        default:
        {{
            {json} = {json}.SkipValue();
            break;
        }}
    }};

    if (propertyIndex != -1)
    {{
        i++;
    }}
}}

{valueVariableName} = new()
{{
    {properties.Select(p => $"{p.Name} = {p.Name.LowerFirstLetter()}").Join(",\n")}
}};

{json} = {json}.SkipOverClosedBracket('}}');

return {json};

}}
");

            return result;
        }

        static string RemoveValueCode(this Compilation compilation,
            ITypeSymbol type,
            PropertyTypeVariant typeKind,
            string variableToAssignValueTo,
            string jsonVariableName = "json")
        {
            var originalType = type;
            var isNullable = originalType.NullableAnnotation == NullableAnnotation.Annotated;
            type = type.UnderNullable();

            string resultVariableOriginal = variableToAssignValueTo;

            bool isNullableValueType = !type.IsReferenceType && isNullable;
            if (isNullableValueType)
            {
                variableToAssignValueTo = $"{variableToAssignValueTo}_Value";
            }

            string result;

            if (typeKind == PropertyTypeVariant.Array)
            {
                type.IsEnumerable(out var underEnumerable);
                var implementationType = compilation.PickValueImplementationType(underEnumerable);
                string v = $"{variableToAssignValueTo}_item";
                string j = $"{jsonVariableName}";
                var deserializeItemMethod = compilation.RemoveValueCode(implementationType.Item2, implementationType.Item1, v, j);

                string vList = $"{v}_list";

                var arrayOrList = type is INamedTypeSymbol ? "List" : "Array";

                result = $@"
{jsonVariableName} = {jsonVariableName}[1..];
var {vList} = new List<{underEnumerable.ToDisplayString()}>();  
while (true)
{{
    {jsonVariableName} = {jsonVariableName}.SkipInsignificantSymbolsLeft();
    var {variableToAssignValueTo}_c = {jsonVariableName}[0];
    if ({variableToAssignValueTo}_c == ']')
    {{
        {jsonVariableName} = {jsonVariableName}[1..];
        break;
    }}
    else if ({variableToAssignValueTo}_c == ',')
    {{
        {jsonVariableName} = {jsonVariableName}[1..];
    }}
    
    {underEnumerable.ToDisplayString()} {v};

    {deserializeItemMethod}

    {vList}.Add({v});
}}
{variableToAssignValueTo} = {vList}{(arrayOrList == "Array" ? ".ToArray()" : "")};
";
            }
            else if (typeKind == PropertyTypeVariant.Dictionary)
            {
                type.IsDictionary(out var keyType, out var valueType);
                var keyImplementationType = compilation.PickValueImplementationType(keyType);
                var valueImplementationType = compilation.PickValueImplementationType(valueType);

                string v = $"{variableToAssignValueTo}_value";
                string k = $"{variableToAssignValueTo}_key";
                string j = $"{jsonVariableName}";

                var deserializeKey = compilation.RemoveValueCode(keyImplementationType.Item2, keyImplementationType.Item1, k, j);
                var deserializeValue = compilation.RemoveValueCode(valueImplementationType.Item2, valueImplementationType.Item1, v, j);

                result = $@"
{jsonVariableName} = {jsonVariableName}.EnterObject();
{variableToAssignValueTo} = new();
while (true)
{{
    {jsonVariableName} = {jsonVariableName}.SkipInsignificantSymbolsLeft();
    var {variableToAssignValueTo}_c = {jsonVariableName}[0];
    if ({variableToAssignValueTo}_c == '}}')
    {{
        {jsonVariableName} = {jsonVariableName}[1..];
        break;
    }}
    else if ({variableToAssignValueTo}_c == ',')
    {{
        json = json[1..];
    }}
    
    {keyType.ToDisplayString()} {k};
    {valueType.ToDisplayString()} {v};

    {deserializeKey}

    {jsonVariableName} = {jsonVariableName}.SkipToPropertyValue();

    {deserializeValue}

    {variableToAssignValueTo}.Add({k}, {v});
}}";
            }
            else if (typeKind == PropertyTypeVariant.Object)
            {
                result = $"{jsonVariableName} = {jsonVariableName}.RemoveObject(out {variableToAssignValueTo});";
            }
            else
            {
                result = $"{jsonVariableName} = {jsonVariableName}.Remove(out {variableToAssignValueTo});";
            }

            if (originalType.IsReferenceType || isNullable)
            {
                result = $@"
if ({jsonVariableName}.TryReadNull(out {resultVariableOriginal})) 
{{
    {jsonVariableName} = {jsonVariableName}[4..];
}}
{{
    {(isNullableValueType ? $"{type.ToDisplayString()} {variableToAssignValueTo}" : "")};
    {result}
    {(isNullableValueType ? $"{resultVariableOriginal} = {variableToAssignValueTo};" : "")}
}}";
            }

            return result;
        }

        public static (PropertyTypeVariant, ITypeSymbol) PickValueImplementationType(this Compilation compilation, ITypeSymbol type)
        {
            var originalType = type;
            type = type.UnderNullable();
            if (type is INamedTypeSymbol nts)
            {
                if (!nts.IsAbstract && !(nts.TypeKind == TypeKind.Interface))
                {
                    var propertyTypeKind =
                        nts.Is<string>() ? PropertyTypeVariant.String :
                        nts.IsNumber() ? PropertyTypeVariant.Number :
                        nts.Is<bool>() ? PropertyTypeVariant.Boolean :
                        nts.ValueIsInQuotes() ? (nts.Is<Char>() ? PropertyTypeVariant.Char : PropertyTypeVariant.ParsableQuoted) : PropertyTypeVariant.None;

                    if (propertyTypeKind != PropertyTypeVariant.None)
                    {
                        return (propertyTypeKind, originalType);
                    }

                    if (nts.IsDictionary(out var key, out var value))
                    {
                        var dictType = nts.ConstructedFrom.Construct(key, value);
                        return (PropertyTypeVariant.Dictionary, dictType);
                    }

                    if (nts.IsEnumerable(out var underEnumerable))
                    {
                        var enumerableType = compilation.GetTypeByMetadataName($"System.Collections.Generic.List<{underEnumerable.ToDisplayString()}>");
                        return (PropertyTypeVariant.Array, enumerableType);
                    }

                    return (PropertyTypeVariant.Object, nts);
                }

                throw new NotImplementedException("Interface or abstract members are not ready yet.");
            }

            if (type.IsEnumerable(out var underEnumerable1)) // is an array
            {
                var arrayType = compilation.CreateArrayTypeSymbol(underEnumerable1);
                return (PropertyTypeVariant.Array, arrayType);
            }

            return (PropertyTypeVariant.Object, type);
        }

        public enum PropertyTypeVariant
        {
            None,
            String,
            Number,
            Boolean,
            ParsableQuoted,
            Object,
            Array,
            Dictionary,
            Char
        }
    }
}
