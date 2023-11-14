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
    {json} = {json}
        .SkipToPropertyName()
        .RemovePropertyName(out var propertyName)
        .SkipToPropertyValue();

    short propertyIndex = propertyName switch 
    {{
        {properties.Select((p, i) => $"\"{(settings.LowerPropertyCase ? p.Name.LowerFirstLetter() : p.Name)}\" => {i},").Join("\n")}
        _ => -1
    }};

    {json} = propertyIndex switch 
    {{
        {properties.Select((p, i) => $"{i} => {compilation.PickValueRemovalMethod(p.Type, p.PropertyTypeVariant, p.Name.LowerFirstLetter(), json)},").Join("\n")}
        _ => {json}.SkipValue()
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

        static string PickValueRemovalMethod(this Compilation compilation,
            ITypeSymbol type,
            PropertyTypeVariant typeKind,
            string variableToAssignValueTo,
            string jsonVariableName = "json")
        {
            if (typeKind == PropertyTypeVariant.String)
            {
                return $"{jsonVariableName}.RemoveQuotedValue(j => new string(j), out {variableToAssignValueTo})";
            }
            if (typeKind == PropertyTypeVariant.Number)
            {
                return $"{jsonVariableName}.RemovePrimitiveValue<{type.ToDisplayString()}>(j => {type.ToDisplayString()}.Parse(j), out {variableToAssignValueTo})";
            }
            if (typeKind == PropertyTypeVariant.Boolean)
            {
                return $"{jsonVariableName}.RemovePrimitiveValue<bool>(j => bool.Parse(j), out {variableToAssignValueTo})";
            }
            if (typeKind == PropertyTypeVariant.DateTime)
            {
                return $"{jsonVariableName}.RemoveQuotedValue(j => DateTime.Parse(j), out {variableToAssignValueTo})";
            }
            if (typeKind == PropertyTypeVariant.Object)
            {
                return $"{jsonVariableName}.RemoveObject(out {variableToAssignValueTo})";
            }
            if (typeKind == PropertyTypeVariant.Array)
            {
                type.IsEnumerable(out var underEnumerable);
                var implementationType = compilation.PickValueImplementationType(underEnumerable);
                string v = $"{variableToAssignValueTo}_{variableToAssignValueTo}";
                string j = jsonVariableName + jsonVariableName;
                var deserializeItemMethod = compilation.PickValueRemovalMethod(implementationType.Item2, implementationType.Item1, v, j);

                return $"{jsonVariableName}.RemoveArray((JsonPiece {j}, out {underEnumerable.ToDisplayString()} {v}) => {{ var r = {deserializeItemMethod}; return r; }}, out {variableToAssignValueTo})";
            }
            if (typeKind == PropertyTypeVariant.Dictionary)
            {
                type.IsDictionary(out var keyType, out var valueType);
                var keyImplementationType = compilation.PickValueImplementationType(keyType);
                var valueImplementationType = compilation.PickValueImplementationType(valueType);

                string v = $"{variableToAssignValueTo}_{variableToAssignValueTo}";
                string j = $"{jsonVariableName}_{jsonVariableName}";

                var deserializeKeyMethod = compilation.PickValueRemovalMethod(keyImplementationType.Item2, keyImplementationType.Item1, v, j);
                var deserializeValueMethod = compilation.PickValueRemovalMethod(valueImplementationType.Item2, valueImplementationType.Item1, v, j);

                return $@"{jsonVariableName}.RemoveDictionary( 
(JsonPiece {j}, out {keyImplementationType.Item2.ToDisplayString()} {v}) => {{ var r = {deserializeKeyMethod}; return r; }},
(JsonPiece {j}, out {valueImplementationType.Item2.ToDisplayString()} {v}) => {{ var r = {deserializeValueMethod}; return r; }}, 
out {variableToAssignValueTo})";
            }

            throw new NotImplementedException();
        }

        public static (PropertyTypeVariant, ITypeSymbol) PickValueImplementationType(this Compilation compilation, ITypeSymbol type)
        {
            if (type is INamedTypeSymbol nts)
            {
                if (!nts.IsAbstract && !(nts.TypeKind == TypeKind.Interface))
                {
                    var propertyTypeKind =
                        nts.Is<string>() ? PropertyTypeVariant.String :
                        nts.IsNumber() ? PropertyTypeVariant.Number :
                        nts.Is<bool>() ? PropertyTypeVariant.Boolean :
                        nts.Is<DateTime>() ? PropertyTypeVariant.DateTime : PropertyTypeVariant.None;

                    if (propertyTypeKind != PropertyTypeVariant.None)
                    {
                        return (propertyTypeKind, type);
                    }

                    if (nts.IsDictionary(out var key, out var value))
                    {
                        var dictType = compilation.GetTypeByMetadataName($"System.Collections.Generic.Dictionary<{key.ToDisplayString()}, {value.ToDisplayString()}>");
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
            DateTime,
            Object,
            Array,
            Dictionary
        }
    }
}
