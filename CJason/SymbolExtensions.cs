using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CJason
{
    public static class SymbolExtensions
    {
        public static IReadOnlyList<Type> PrimitiveTypes =>
            new Type[] { typeof(string), typeof(bool), typeof(DateTime), typeof(TimeSpan) }
                .Concat(NumberTypes)
                .ToArray();

        public static readonly IReadOnlyList<Type> NumberTypes = new Type[]
        {
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(double),
            typeof(float),
            typeof(decimal)
        };

        public static bool IsNumber(this ITypeSymbol type) => NumberTypes.Any(n => type.Is(n));

        public static bool IsPrimitive(this ITypeSymbol type)
        {
            for (int i = 0; i < PrimitiveTypes.Count; i++)
            {
                if (type.Is(PrimitiveTypes[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Is<T>(this ITypeSymbol type) => type.Is(typeof(T));

        static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.None,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
        );

        public static bool Is(this ITypeSymbol type, Type t)
        {
            var signature = type.ToDisplayString(_symbolDisplayFormat);
            var givenSignature = t.FullName;

            return givenSignature == signature;
        }

        public static bool IsEnumerable(this ITypeSymbol type, out ITypeSymbol underEnumerable)
        {
            underEnumerable = null;
            if (type.Is<string>())
            {
                return false;
            }
            var enumerableInterface = type.AllInterfaces.FirstOrDefault(
                i => i.ToDisplayString().StartsWith("System.Collections.Generic.IEnumerable<")
            );
            if (enumerableInterface != null)
            {
                underEnumerable = enumerableInterface.TypeArguments.First();
                return true;
            }
            return false;
        }

        public static bool IsDictionary(this ITypeSymbol type, out ITypeSymbol keyType, out ITypeSymbol valueType)
        {
            if (!type.ToDisplayString().StartsWith("System.Collections.Generic.Dictionary<"))
            {
                keyType = null;
                valueType = null;
                return false;
            }

            var nts = type as INamedTypeSymbol;

            keyType = nts.TypeArguments.First();
            valueType = nts.TypeArguments.Last();

            return true;
        }
    }
}
