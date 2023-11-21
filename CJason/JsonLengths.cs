using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CJason
{
    public static class JsonLengths
    {
        public const int Boolean = 5; // Length of "false"
        public const int Char = 3; // Length of "'c'"
        public const int SByte = 4; // Length of "-128"
        public const int Byte = 3; // Length of "255"
        public const int Int16 = 6; // Length of "-32768"
        public const int UInt16 = 5; // Length of "65535"
        public const int Int32 = 11; // Length of "-2147483648"
        public const int UInt32 = 10; // Length of "4294967295"
        public const int Int64 = 20; // Length of "-9223372036854775808"
        public const int UInt64 = 20; // Length of "18446744073709551615"
        public const int Single = 9; // Length of "-3.40282347" (typical float precision)
        public const int Double = 17; // Length of "-1.7976931348623157E+308" (typical double precision)
        public const int Decimal = 29; // Length of "-79228162514264337593543950335" (maximum decimal value)

        public const int DateTime = 21; // Length of '01.01.0001T00.00.00'

        public static readonly IReadOnlyDictionary<string, int> AsDictionary = new Dictionary<
            string,
            int
        >
        {
            { nameof(JsonLengths.Boolean), JsonLengths.Boolean },
            { nameof(JsonLengths.Char), JsonLengths.Char },
            { nameof(JsonLengths.SByte), JsonLengths.SByte },
            { nameof(JsonLengths.Byte), JsonLengths.Byte },
            { nameof(JsonLengths.Int16), JsonLengths.Int16 },
            { nameof(JsonLengths.UInt16), JsonLengths.UInt16 },
            { nameof(JsonLengths.Int32), JsonLengths.Int32 },
            { nameof(JsonLengths.UInt32), JsonLengths.UInt32 },
            { nameof(JsonLengths.Int64), JsonLengths.Int64 },
            { nameof(JsonLengths.UInt64), JsonLengths.UInt64 },
            { nameof(JsonLengths.Single), JsonLengths.Single },
            { nameof(JsonLengths.Double), JsonLengths.Double },
            { nameof(JsonLengths.Decimal), JsonLengths.Decimal },

            { nameof(System.DateTime), 30 },
            { nameof(TimeSpan), 15 },
            { nameof(DateTimeOffset), 36 }
        };

        static readonly SymbolDisplayFormat OnlyTypeName = new SymbolDisplayFormat(
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.None,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly
        );

        public static int GetConstantLength(this ITypeSymbol type)
        {
            var typeName = type.ToDisplayString(OnlyTypeName);
            var dict = JsonLengths.AsDictionary;

            if (dict.ContainsKey(typeName))
            {
                return dict[typeName];
            }

            return -1;
        }

        public static readonly string CalculateExtensionsClass =
            $@"
public static class JsonLengthExtensions
{{    
    {AsDictionary.Select(kv => $"public static int CalculateJsonLength(this System.{kv.Key} _) => {kv.Value};").Join("\n")}

    {AsDictionary.Select(kv => $"public static int CalculateJsonLength(this System.Span<System.{kv.Key}> numbers) => (items.Length + 1) * {kv.Value} - 1;").Join("\n")}

    {AsDictionary.Select(kv => $"public static int CalculateJsonLength(this System.Collections.Generic.IEnumerable<System.{kv.Key}> numbers) => numbers.EnsureSpan().CalculateJsonLength();").Join("\n")}

    [System.Runtime.CompilerServices.MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
    public static int CalculateJsonLength(this string str) => str?.Length ?? 0 + 2;

    public static int CalculateJsonLength(this System.Span<string> strings)
    {{
        var result = 0;
        var length = strings.Length;
        for (int i = 0; i < length; i++)
        {{
            result += strings[i].Length + 3;
        }}
        result--;
        return result;
    }}

    public static int CalculateJsonLength(this System.Collections.Generic.IEnumerable<string> strings) => strings.EnsureSpan().CalculateJsonLength();
}}";
    }
}
