using CJason;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.IO.Directory;

namespace GenTests;

public class Generate
{
    static readonly string _sampleClassCode;

    static Generate()
    {
        var dir = GetParent(GetCurrentDirectory()).Parent.Parent.Parent;
        string sampleClassesFile = $@"{dir}\Artifact\SampleClasses.cs";
        _sampleClassCode = File.ReadAllText(sampleClassesFile);
    }

    [SetUp]
    public void Setup() { }


    [Test]
    public void GenerateCode()
    {
        var tree = CSharpSyntaxTree.ParseText(_sampleClassCode);

        var coreLib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var compilation = CSharpCompilation.Create(
            "SampleCompilation",
            new[] { tree },
            references: new[] { coreLib }
        );

        var model = compilation.GetSemanticModel(tree);

        var classSyntaxes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

        var classes = classSyntaxes
            .Select(c => model.GetDeclaredSymbol(c))
            .SelectMany(c => c.GatherReferredTypes())
            //.Where(t => !t.ContainingNamespace.ToDisplayString().StartsWith("System."))
            .Distinct(EqualityComparer<ITypeSymbol>.Default)
            .ToArray();

        var generated = classes
            .Select(classSymbol =>
                    compilation.GenerateRemoveObjectMethod(classSymbol) +
                    "\n" +
                    SerializationGenerator.GenerateSerializationMethodsEtc(classSymbol)
                )
            .Join("\n");

        generated =
            $@"
using JsonPiece = System.ReadOnlySpan<char>;
using CJason.Provision;
using System;
using System.Collections.Generic;
using System.Linq;


        namespace SampleNamespace 
        {{
            public static class ArtificialStringifier
            {{
                {JsonLengths.AsDictionary.Select(kv =>
               {
                   string typeName = kv.Key;
                   int length = kv.Value;

                   bool valueIsInQuotes = new Type[]
                   {
                       typeof(DateTime), typeof(DateOnly), typeof(TimeSpan), typeof(string), typeof(char)
                   }.Select(t => t.FullName).Contains(typeName);

                   var typeMethods = $@"
                    public static int CalculateJsonLength(this Span<System.{typeName}> consts) => (consts.Length + 1) * {length} + 2;
                    public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, {typeName}>> items)
                    {{
                        var result = 2;
                        foreach (var (key, value) in items)
                        {{
                            result += key.Length * 2 + 2 + 1;
                            result += {length};
                        }}
                        return result == 3 ? 2 : result;
                    }}

                    public static Span<char> {SerializationGenerator.FillWithMethodName}(this Span<char> span, Span<{typeName}> items)
                    {{
                        span[0] = '[';
                        span = span[1..];
                        var length = items.Length;
                        for (int i = 0; i < length; i++)
                        {{
                            if (i > 0)
                            {{
                                span[0] = ',';
                                span = span[1..];
                            }}
                            var itemString = items[i].ToString().AsSpan();
                            span = span[itemString.CopiedTo(span)..];
                        }}
                        span[0] = ']';
                        return span[1..];
                    }}

                    public static Span<char> {SerializationGenerator.FillWithMethodName}(this Span<char> json, IEnumerable<KeyValuePair<string, {typeName}>> items)
                    {{
                        json[0] = '{{';
                        json = json[1..];
                        bool isFirst = true;
                        foreach (var (key, value) in items)
                        {{
                            if (!isFirst)
                            {{
                                json[0] = ',';
                                json = json[1..];
                            }}
                            else 
                            {{
                                isFirst = false;
                            }}
                            json[0] = '""';
                            json = json[1..];
                            ReadOnlySpan<char> keySpan = key;
                            keySpan.CopyTo(json);
                            json = json[keySpan.Length..];                            
                            json[0] = '""';
                            json[1] = ':';
                            json = json[2..];
                            {(valueIsInQuotes ? $@"
    valueSpan[0] = '""';
    ReadOnlySpan<char> valueSpan = value.ToString();
    valueSpan.CopyTo(json);
    json = json[valueSpan.Length..];
    json[0] = '""';
    json = json[1..];
" : $@"
    ReadOnlySpan<char> valueSpan = value.ToString();
    valueSpan.CopyTo(json);
    json = json[valueSpan.Length..];")}
                        }}

                        json[0] = '}}';
                        json = json[1..];
                        return json;
                    }}
";
                   return typeMethods;
               }).Join("\n")}

                

                public static Span<T> EnsureSpan<T>(this IEnumerable<T> source)
                {{
                    if (source is T[] span)
                    {{
                        return span;
                    }}
                    if (source is List<T> list)
                    {{
                        return System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
                    }}
                    var array = source.ToArray();
                    return array;
                }}

                public static int CopiedTo<T>(this Span<T> source, Span<T> target)
                {{
                    source.CopyTo(target);
                    return source.Length;
                }}

                public static int CopiedTo<T>(this ReadOnlySpan<T> source, Span<T> target)
                {{
                    source.CopyTo(target);
                    return source.Length;
                }}

                {generated}
            }}
        }}";

        File.WriteAllText("..\\..\\..\\..\\Artifact\\Code.cs", generated);

        return;
    }
}
